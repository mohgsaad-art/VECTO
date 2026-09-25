"""
Tests for FUNCTIONS.utils_generate_vecto_input2run: is_xml_valid2run and its
supporting reductions (get_vehicles, get_REF_vehicles).
"""

import json
import os
import sys
from datetime import datetime

import pandas as pd
import pytest

sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..'))
from FUNCTIONS.utils_generate_vecto_input2run import (
    generate_PRIM_COMP_job,
    generate_PRIMVIF_COMP_job,
    get_REF_vehicles,
    get_vehicles,
    read_vecto_file,
)

_NS = 'xmlns:tns="urn:tugraz:ivt:VectoAPI:DeclarationInput:v2.0" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"'

_VALID_HVAC_BLOCK = """
        <HVAC>
            <SystemConfiguration>0</SystemConfiguration>
            <HeatPumpTypeDriverCompartment>
                <Cooling>none</Cooling>
                <Heating>none</Heating>
            </HeatPumpTypeDriverCompartment>
            <HeatPumpTypePassengerCompartment>
                <Cooling>none</Cooling>
                <Heating>none</Heating>
            </HeatPumpTypePassengerCompartment>
            <DoubleGlazing>false</DoubleGlazing>
            <AdjustableAuxiliaryHeater>false</AdjustableAuxiliaryHeater>
            <AirElectricHeater>false</AirElectricHeater>
            <OtherHeatingTechnology>false</OtherHeatingTechnology>
            <SeparateAirDistributionDucts>false</SeparateAirDistributionDucts>
        </HVAC>
"""


def _completed_bus_xml(
    *,
    conventional: bool = True,
    declaration_type: str = 'final',
    corrected_actual_mass: str = '500',
    tpmlm: str = '3500',
    seats_lower: str = '1',
    standing_lower: str = '10',
    seats_upper: str = '11',
    standing_upper: str = '2',
    omit_field: str | None = None,
    hvac_block: str = _VALID_HVAC_BLOCK,
    omit_hvac: bool = False,
    hvac_field_omit: str | None = None,
) -> str:
    vtype = 'Vehicle_Conventional_CompletedBusDeclarationType' if conventional else 'Vehicle_PEV_CompletedBusDeclarationType'
    fields = {
        'CorrectedActualMass': corrected_actual_mass,
        'TechnicalPermissibleMaximumLadenMass': tpmlm,
        'NumberPassengerSeatsLowerDeck': seats_lower,
        'NumberPassengersStandingLowerDeck': standing_lower,
        'NumberPassengerSeatsUpperDeck': seats_upper,
        'NumberPassengersStandingUpperDeck': standing_upper,
    }
    field_xml = ''.join(
        f'<{name}>{value}</{name}>\n' for name, value in fields.items() if name != omit_field
    )

    ev_fields = ''
    if not conventional:
        ev_names = [
            'ElectricSystem', 'LEDLights', 'DoubleGlazing',
            'AdjustableAuxiliaryHeater', 'AirElectricHeater',
            'OtherHeatingTechnology', 'SeparateAirDistributionDucts',
        ]
        ev_fields = ''.join(f'<{n}>true</{n}>\n' for n in ev_names if n != omit_field)

    hvac_xml = '' if omit_hvac else hvac_block
    # The xEV-required-field check (xml_tree.find(".//{*}" + fld)) searches
    # the whole document, and several of those field names (DoubleGlazing,
    # AdjustableAuxiliaryHeater, AirElectricHeater, OtherHeatingTechnology,
    # SeparateAirDistributionDucts) legitimately also appear inside <HVAC>
    # in real declaration XML - so omitting a field to test "missing xEV
    # field" must strip it from the HVAC block too, or the document-wide
    # search still finds the HVAC copy and the test would not actually
    # exercise the missing-field path.
    strip_from_hvac = omit_field or hvac_field_omit
    if strip_from_hvac:
        hvac_xml = '\n'.join(
            line for line in hvac_xml.splitlines() if strip_from_hvac not in line
        )

    return f"""<?xml version="1.0" encoding="utf-8"?>
<tns:VectoInputDeclaration {_NS}>
    <tns:Vehicle xsi:type="{vtype}">
        <VIN>SOME-VIN</VIN>
        <Date>2021-06-30T22:00:00Z</Date>
        <Manufacturer>manufacturer_name</Manufacturer>
        <LegislativeCategory>M3</LegislativeCategory>
        {field_xml}
        {ev_fields}
        <VehicleDeclarationType>{declaration_type}</VehicleDeclarationType>
        {hvac_xml}
    </tns:Vehicle>
</tns:VectoInputDeclaration>
"""


def _primary_bus_xml() -> str:
    return f"""<?xml version="1.0" encoding="utf-8"?>
<tns:VectoInputDeclaration {_NS}>
    <tns:Vehicle xsi:type="Vehicle_Conventional_PrimaryBusDeclarationType">
        <VIN>SOME-VIN</VIN>
        <Date>2021-06-30T22:00:00Z</Date>
        <Manufacturer>manufacturer_name</Manufacturer>
        <LegislativeCategory>M3</LegislativeCategory>
    </tns:Vehicle>
</tns:VectoInputDeclaration>
"""


def _lorry_xml() -> str:
    return f"""<?xml version="1.0" encoding="utf-8"?>
<tns:VectoInputDeclaration {_NS}>
    <tns:Vehicle xsi:type="Vehicle_Conventional_MediumLorryDeclarationType">
        <VIN>SOME-VIN</VIN>
        <Date>2021-06-30T22:00:00Z</Date>
        <Manufacturer>manufacturer_name</Manufacturer>
        <LegislativeCategory>N2</LegislativeCategory>
    </tns:Vehicle>
</tns:VectoInputDeclaration>
"""


def _hvac_config6(driver_has_pump: bool, passenger_has_pump: bool) -> str:
    driver = 'non R-744 continuous' if driver_has_pump else 'none'
    passenger = 'non R-744 continuous' if passenger_has_pump else 'none'
    return f"""
        <HVAC>
            <SystemConfiguration>6</SystemConfiguration>
            <HeatPumpTypeDriverCompartment>
                <Cooling>{driver}</Cooling>
                <Heating>none</Heating>
            </HeatPumpTypeDriverCompartment>
            <HeatPumpTypePassengerCompartment>
                <Cooling>{passenger}</Cooling>
                <Heating>none</Heating>
            </HeatPumpTypePassengerCompartment>
            <DoubleGlazing>false</DoubleGlazing>
            <AdjustableAuxiliaryHeater>false</AdjustableAuxiliaryHeater>
            <AirElectricHeater>false</AirElectricHeater>
            <OtherHeatingTechnology>false</OtherHeatingTechnology>
            <SeparateAirDistributionDucts>false</SeparateAirDistributionDucts>
        </HVAC>
"""


class TestIsXmlValid2Run:
    def test_valid_completed_conventional_bus_passes_all_checks(self, tmp_path):
        f = tmp_path / 'comp.xml'
        f.write_text(_completed_bus_xml(conventional=True), encoding='utf-8')

        result = read_vecto_file(f)

        assert result['is_xml_valid2run'] is True
        assert result['is_xml_completed'] is True

    def test_missing_mass_field_fails(self, tmp_path):
        f = tmp_path / 'comp.xml'
        f.write_text(_completed_bus_xml(conventional=True, omit_field='TechnicalPermissibleMaximumLadenMass'), encoding='utf-8')

        result = read_vecto_file(f)

        assert result['is_xml_valid2run'] is False

    def test_mass_exceeding_gvm_fails(self, tmp_path):
        f = tmp_path / 'comp.xml'
        f.write_text(
            _completed_bus_xml(conventional=True, corrected_actual_mass='3400', tpmlm='3500',
                                seats_lower='0', standing_lower='0', seats_upper='0', standing_upper='2'),
            encoding='utf-8',
        )
        # mass + passengers*68 = 3400 + 2*68 = 3536 > 3500 -> should fail
        result = read_vecto_file(f)

        assert result['is_xml_valid2run'] is False

    def test_non_conventional_missing_xev_field_fails(self, tmp_path):
        f = tmp_path / 'comp.xml'
        f.write_text(_completed_bus_xml(conventional=False, omit_field='DoubleGlazing'), encoding='utf-8')

        result = read_vecto_file(f)

        assert result['is_xml_valid2run'] is False

    def test_interim_declaration_type_fails(self, tmp_path):
        f = tmp_path / 'comp.xml'
        f.write_text(_completed_bus_xml(conventional=False, declaration_type='interim'), encoding='utf-8')

        result = read_vecto_file(f)

        assert result['is_xml_valid2run'] is False

    def test_hvac_config6_with_disallowed_driver_heat_pump_fails(self, tmp_path):
        f = tmp_path / 'comp.xml'
        f.write_text(
            _completed_bus_xml(conventional=True, hvac_block=_hvac_config6(driver_has_pump=True, passenger_has_pump=True)),
            encoding='utf-8',
        )

        result = read_vecto_file(f)

        assert result['is_xml_valid2run'] is False

    def test_hvac_config6_valid_passenger_only_heat_pump_passes(self, tmp_path):
        f = tmp_path / 'comp.xml'
        f.write_text(
            _completed_bus_xml(conventional=True, hvac_block=_hvac_config6(driver_has_pump=False, passenger_has_pump=True)),
            encoding='utf-8',
        )

        result = read_vecto_file(f)

        assert result['is_xml_valid2run'] is True

    def test_primary_only_file_is_valid2run_without_mass_or_hvac_checks(self, tmp_path):
        f = tmp_path / 'prim.xml'
        f.write_text(_primary_bus_xml(), encoding='utf-8')

        result = read_vecto_file(f)

        assert result['is_xml_completed'] is False
        assert result['is_xml_valid2run'] is True

    def test_lorry_declaration_type_is_flagged(self, tmp_path):
        f = tmp_path / 'lorry.xml'
        f.write_text(_lorry_xml(), encoding='utf-8')

        result = read_vecto_file(f)

        assert result['is_xml_lorry'] is True
        assert result['is_xml_valid2run'] is True

    def test_primary_bus_is_not_flagged_as_lorry(self, tmp_path):
        f = tmp_path / 'prim.xml'
        f.write_text(_primary_bus_xml(), encoding='utf-8')

        result = read_vecto_file(f)

        assert result['is_xml_lorry'] is False


class TestGetVehiclesValidToRunReduction:
    @staticmethod
    def _base_row(**overrides):
        row = {
            'IssueVIN': '0001_VIN1',
            'vehicle_xsd_type': 'DeclarationInput:VectoInputDeclaration/x',
            'file_type': 1,
            'local_path': 'p',
            'isDeclared': True,
            'is_xml_conventional': True,
            'is_xml_fchv': False,
            'is_xml_hev': False,
            'is_xml_iepc': False,
            'is_xml_pev': False,
            'is_xml_ovc': False,
            'is_xml_lorry': False,
            'is_xml_multi': False,
            'veh_class': 'M3',
            'veh_category': 'cat',
            'OEM': 'oem',
            'VIN': 'VIN1',
            'iid': 1,
            'art10_label': 'art10::accepted',
            'data_hash': 'hash1',
            'data_hash_short': 'ha1',
            'is_xml_valid2run': True,
        }
        row.update(overrides)
        return row

    def test_primary_plus_complete_pair_uses_complete_files_validity(self):
        # vehicle_xsd_type must differ between the two files - get_vehicles
        # groups by (IssueVIN, vehicle_xsd_type).tail(1) first, which would
        # otherwise collapse two same-typed rows for one IssueVIN into one.
        primary = self._base_row(file_type=1, is_xml_valid2run=True,
                                  vehicle_xsd_type='DeclarationInput:VectoInputDeclaration/Primary')
        complete = self._base_row(file_type=3, is_xml_valid2run=False,
                                   vehicle_xsd_type='DeclarationInput:VectoInputDeclaration/Complete')
        df = pd.DataFrame([primary, complete])

        result = get_vehicles(df)

        assert result.loc['0001_VIN1', 'is_xml_valid2run'] == False

    def test_lorry_flag_survives_reduction_as_scalar(self):
        primary = self._base_row(file_type=1, is_xml_lorry=True)
        df = pd.DataFrame([primary])

        result = get_vehicles(df)

        # .str[0] on a list-of-bool column yields a proper pandas bool-dtype
        # Series (element access returns numpy.bool_, pandas' normal boxed
        # scalar type for a bool-dtype column) - this is NOT the numpy-
        # scalar-leak bug fixed elsewhere in this module (that one was about
        # object-dtype columns holding raw numpy scalars from
        # list(np.array(...)), which serialize to Excel as "np.True_"/
        # "np.float64(...)"; a genuine bool-dtype Series serializes cleanly
        # as True/False). Confirmed via a real to_excel/read_excel round-trip.
        assert bool(result.loc['0001_VIN1', 'is_xml_lorry']) is True
        assert result['is_xml_lorry'].dtype == bool


class TestGetREFVehiclesValidToRunSurvivesDedup:
    def test_ref_idx_selects_matching_valid2run_value(self):
        # Two duplicate-hash vehicles: the declared one (selected as REF_idx)
        # is valid2run=True, the undeclared one is False. REF_is_xml_valid2run
        # must reflect the SELECTED vehicle's own value, not just any truthy
        # value from the group.
        df_vehicles = pd.DataFrame([
            {
                'veh_hash': 'H1', 'local_path': 'path_a', 'file_type': 1,
                'Issue_VIN': '0001_VINA', 'vehicle_type': 'PrimaryBus',
                'veh_category': 'CategoryA', 'veh_pwt': 'CONV',
                'veh_hash_short': 'H1sh', 'veh_Declared': True,
                'art10_label': 'art10::accepted', 'is_xml_valid2run': True,
            },
            {
                'veh_hash': 'H1', 'local_path': 'path_b', 'file_type': 1,
                'Issue_VIN': '0002_VINB', 'vehicle_type': 'PrimaryBus',
                'veh_category': 'CategoryA', 'veh_pwt': 'CONV',
                'veh_hash_short': 'H1sh', 'veh_Declared': False,
                'art10_label': 'art10::accepted', 'is_xml_valid2run': False,
            },
        ])

        result = get_REF_vehicles(df_vehicles)

        assert result.loc['H1', 'REF_is_xml_valid2run'] == True
        assert result.loc['H1', 'REF_local_path'] == 'path_a'


class TestA02RunSimulationFlag:
    def test_generate_prim_comp_job_run_simulation_false(self, tmp_path):
        out = tmp_path / 'job.vecto'
        generate_PRIM_COMP_job('prim.xml', 'comp.xml', str(out), RunSimulation=False)

        content = out.read_text(encoding='utf-8')
        assert '"RunSimulation": "false"' in content

    def test_generate_prim_comp_job_default_run_simulation_true(self, tmp_path):
        out = tmp_path / 'job.vecto'
        generate_PRIM_COMP_job('prim.xml', 'comp.xml', str(out))

        content = out.read_text(encoding='utf-8')
        assert '"RunSimulation": "true"' in content

    def test_generate_primvif_comp_job_run_simulation_false(self, tmp_path):
        out = tmp_path / 'job.vecto'
        generate_PRIMVIF_COMP_job('vif.xml', 'comp.xml', str(out), RunSimulation=False)

        content = out.read_text(encoding='utf-8')
        assert '"RunSimulation": "false"' in content

    def test_generate_primvif_comp_job_default_run_simulation_true(self, tmp_path):
        out = tmp_path / 'job.vecto'
        generate_PRIMVIF_COMP_job('vif.xml', 'comp.xml', str(out))

        content = out.read_text(encoding='utf-8')
        assert '"RunSimulation": "true"' in content

    def test_generate_prim_comp_job_uses_supplied_datetime(self, tmp_path):
        out = tmp_path / 'job.vecto'
        fixed = datetime(2026, 1, 8, 8, 30, 0, 8888)
        generate_PRIM_COMP_job('prim.xml', 'comp.xml', str(out), datetime_val=fixed)

        job = json.loads(out.read_text(encoding='utf-8'))
        assert job['Header']['Date'] == fixed.isoformat() + 'Z'

    def test_generate_prim_comp_job_defaults_to_now(self, tmp_path):
        out = tmp_path / 'job.vecto'
        before = datetime.now()
        generate_PRIM_COMP_job('prim.xml', 'comp.xml', str(out))
        after = datetime.now()

        job = json.loads(out.read_text(encoding='utf-8'))
        written = datetime.fromisoformat(job['Header']['Date'].removesuffix('Z'))
        assert before <= written <= after

    def test_generate_primvif_comp_job_uses_supplied_datetime(self, tmp_path):
        out = tmp_path / 'job.vecto'
        fixed = datetime(2026, 1, 8, 8, 30, 0, 8888)
        generate_PRIMVIF_COMP_job('vif.xml', 'comp.xml', str(out), datetime_val=fixed)

        job = json.loads(out.read_text(encoding='utf-8'))
        assert job['Header']['Date'] == fixed.isoformat() + 'Z'

    def test_generate_primvif_comp_job_defaults_to_now(self, tmp_path):
        out = tmp_path / 'job.vecto'
        before = datetime.now()
        generate_PRIMVIF_COMP_job('vif.xml', 'comp.xml', str(out))
        after = datetime.now()

        job = json.loads(out.read_text(encoding='utf-8'))
        written = datetime.fromisoformat(job['Header']['Date'].removesuffix('Z'))
        assert before <= written <= after
