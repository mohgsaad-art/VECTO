using System;
using System.Diagnostics;

namespace TUGraz.VectoCommon.Utils
{
    public static class ErrorCodes
    {
        public static readonly ErrorCode ADAS_ECOROLL_INVALID_COMBINATION = new ErrorCode(nameof(ADAS_ECOROLL_INVALID_COMBINATION));
        public static readonly ErrorCode ADAS_ECOROLL_RELEASE_LOCKUP_CLUTCH_REQUIRED = new ErrorCode(nameof(ADAS_ECOROLL_RELEASE_LOCKUP_CLUTCH_REQUIRED));
        public static readonly ErrorCode ADAS_INVALID_COMBINATION = new ErrorCode(nameof(ADAS_INVALID_COMBINATION));
        public static readonly ErrorCode ADAS_MISSING_STOPSTART = new ErrorCode(nameof(ADAS_MISSING_STOPSTART));

        public static readonly ErrorCode AIRDRAG_ENTRIES_DONT_COVER_BETA = new ErrorCode(nameof(AIRDRAG_ENTRIES_DONT_COVER_BETA));
        public static readonly ErrorCode AIRDRAG_CROSSWIND_CORRECTION_EXTRAPOLATION = new ErrorCode(nameof(AIRDRAG_CROSSWIND_CORRECTION_EXTRAPOLATION));
        public static readonly ErrorCode AIRDRAG_MISSING_IMPLEMENTATION = new ErrorCode(nameof(AIRDRAG_MISSING_IMPLEMENTATION));

        public static readonly ErrorCode ANGLEDRIVE_LOSS_MAP_NOT_FOUND = new ErrorCode(nameof(ANGLEDRIVE_LOSS_MAP_NOT_FOUND));

        public static readonly ErrorCode AUXILIARIES_INVALID_HVAC_CONFIGURATION = new ErrorCode(nameof(AUXILIARIES_INVALID_HVAC_CONFIGURATION));
        public static readonly ErrorCode AUXILIARIES_MISSING_HVAC_CONFIGURATION = new ErrorCode(nameof(AUXILIARIES_MISSING_HVAC_CONFIGURATION));
        public static readonly ErrorCode AUXILIARIES_MISSING_POWER_DEMAND = new ErrorCode(nameof(AUXILIARIES_MISSING_POWER_DEMAND));
        public static readonly ErrorCode AUXILIARIES_INVALID_COMBINATION = new ErrorCode(nameof(AUXILIARIES_INVALID_COMBINATION));
        public static readonly ErrorCode AUXILIARIES_MISSING_TYPE = new ErrorCode(nameof(AUXILIARIES_MISSING_TYPE));
        public static readonly ErrorCode AUXILIARIES_WRONG_VEHICLE_TYPE = new ErrorCode(nameof(AUXILIARIES_WRONG_VEHICLE_TYPE));
        public static readonly ErrorCode AUXILIARIES_MISSING_PARAMETER = new ErrorCode(nameof(AUXILIARIES_MISSING_PARAMETER));
        public static readonly ErrorCode AUXILIARIES_MISSING_CONFIGURATION = new ErrorCode(nameof(AUXILIARIES_MISSING_CONFIGURATION));
        public static readonly ErrorCode AUXILIARIES_INPUT_WRONG_MODE = new ErrorCode(nameof(AUXILIARIES_INPUT_WRONG_MODE));
        public static readonly ErrorCode AUXILIARIES_UNEXPECTED_VEHICLE_TYPE = new ErrorCode(nameof(AUXILIARIES_UNEXPECTED_VEHICLE_TYPE));
        public static readonly ErrorCode AUXILIARIES_UNKNOWN_TECHNOLOGY = new ErrorCode(nameof(AUXILIARIES_UNKNOWN_TECHNOLOGY));
        public static readonly ErrorCode AUXILIARIES_INVALID_ID = new ErrorCode(nameof(AUXILIARIES_INVALID_ID));
        public static readonly ErrorCode AUXILIARIES_MISSING_DCDCCONVERTER = new ErrorCode(nameof(AUXILIARIES_MISSING_DCDCCONVERTER));
        public static readonly ErrorCode AUXILIARIES_COMPRESSOR_MAP_MISSING = new ErrorCode(nameof(AUXILIARIES_COMPRESSOR_MAP_MISSING));
        public static readonly ErrorCode AUXILIARIES_WRITER_NOT_INITIALIZED = new ErrorCode(nameof(AUXILIARIES_WRITER_NOT_INITIALIZED));
        public static readonly ErrorCode AUXILIARIES_MISSING_DATASOURCE = new ErrorCode(nameof(AUXILIARIES_MISSING_DATASOURCE));

        public static readonly ErrorCode AXLEGEAR_MISSING_RATIO = new ErrorCode(nameof(AXLEGEAR_MISSING_RATIO));

        public static readonly ErrorCode BRAKES_NEGATIVE_BREAKING_POWER = new ErrorCode(nameof(BRAKES_NEGATIVE_BREAKING_POWER));

        public static readonly ErrorCode CYCLE_TORQUE_CONVERTER_INFO_MISSING = new ErrorCode(nameof(CYCLE_TORQUE_CONVERTER_INFO_MISSING));
        public static readonly ErrorCode CYCLE_STOPPING_TIME_NOT_ALLOWED_WITH_POSITIVE_SPEED = new ErrorCode(nameof(CYCLE_STOPPING_TIME_NOT_ALLOWED_WITH_POSITIVE_SPEED));
        public static readonly ErrorCode CYCLE_NEGATIVE_VEHICLE_VELOCITY = new ErrorCode(nameof(CYCLE_NEGATIVE_VEHICLE_VELOCITY));
        public static readonly ErrorCode CYCLE_NEGATIVE_DELTA_TIME = new ErrorCode(nameof(CYCLE_NEGATIVE_DELTA_TIME));
        public static readonly ErrorCode CYCLE_NEGATIVE_DISTANCE = new ErrorCode(nameof(CYCLE_NEGATIVE_DISTANCE));
        public static readonly ErrorCode CYCLE_FAILED_TO_READ_STREAM = new ErrorCode(nameof(CYCLE_FAILED_TO_READ_STREAM));
        public static readonly ErrorCode CYCLE_FAILED_TO_READ_FILE = new ErrorCode(nameof(CYCLE_FAILED_TO_READ_FILE));
        public static readonly ErrorCode CYCLE_CANNOT_HANDLE_REQUEST = new ErrorCode(nameof(CYCLE_CANNOT_HANDLE_REQUEST));
        public static readonly ErrorCode CYCLE_UNSUPPORTED_TYPE = new ErrorCode(nameof(CYCLE_UNSUPPORTED_TYPE));
        public static readonly ErrorCode CYCLE_INVALID_START_SPEED_AND_ACCELERATION = new ErrorCode(nameof(CYCLE_INVALID_START_SPEED_AND_ACCELERATION));
        public static readonly ErrorCode CYCLE_EXCEEDED = new ErrorCode(nameof(CYCLE_EXCEEDED));

        public static readonly ErrorCode DATATABLE_FAILED_TO_READ_FILE = new ErrorCode(nameof(DATATABLE_FAILED_TO_READ_FILE));
        public static readonly ErrorCode DATATABLE_FAILED_TO_READ_STREAM = new ErrorCode(nameof(DATATABLE_FAILED_TO_READ_STREAM));
        public static readonly ErrorCode DATATABLE_EMPTY_FILE = new ErrorCode(nameof(DATATABLE_EMPTY_FILE));
        public static readonly ErrorCode DATATABLE_WRONG_COLUMNS = new ErrorCode(nameof(DATATABLE_WRONG_COLUMNS));
        public static readonly ErrorCode DATATABLE_INVALID_FILENAME = new ErrorCode(nameof(DATATABLE_INVALID_FILENAME));
        public static readonly ErrorCode DATATABLE_INSUFFICIENT_DATA = new ErrorCode(nameof(DATATABLE_INSUFFICIENT_DATA));
        public static readonly ErrorCode DATATABLE_DUPLICATE_ENTRY = new ErrorCode(nameof(DATATABLE_DUPLICATE_ENTRY));
        public static readonly ErrorCode DATATABLE_DUPLICATE_COLUMN = new ErrorCode(nameof(DATATABLE_DUPLICATE_COLUMN));
        public static readonly ErrorCode DATATABLE_INVALID_DATA = new ErrorCode(nameof(DATATABLE_INVALID_DATA));
        public static readonly ErrorCode DATATABLE_MISSING_COLUMN = new ErrorCode(nameof(DATATABLE_MISSING_COLUMN));

        public static readonly ErrorCode DECLARATIONDATA_GROUP_NOT_FOUND = new ErrorCode(nameof(DECLARATIONDATA_GROUP_NOT_FOUND));
        public static readonly ErrorCode DECLARATIONDATA_SEGMENT_NOT_FOUND = new ErrorCode(nameof(DECLARATIONDATA_SEGMENT_NOT_FOUND));
        public static readonly ErrorCode DECLARATIONDATA_FUEL_NOT_FOUND = new ErrorCode(nameof(DECLARATIONDATA_FUEL_NOT_FOUND));
        public static readonly ErrorCode DECLARATIONDATA_UNSUPPORTED_LOADING = new ErrorCode(nameof(DECLARATIONDATA_UNSUPPORTED_LOADING));
        public static readonly ErrorCode DECLARATIONDATA_MISSING_DATA = new ErrorCode(nameof(DECLARATIONDATA_MISSING_DATA));
        public static readonly ErrorCode DECLARATIONDATA_INVALID_VEHICLE = new ErrorCode(nameof(DECLARATIONDATA_INVALID_VEHICLE));
        public static readonly ErrorCode DECLARATIONDATA_INVALID_COMPRESSOR_SIZE = new ErrorCode(nameof(DECLARATIONDATA_INVALID_COMPRESSOR_SIZE));
        public static readonly ErrorCode DECLARATIONDATA_INVALID_AXLES_COUNT = new ErrorCode(nameof(DECLARATIONDATA_INVALID_AXLES_COUNT));
        public static readonly ErrorCode DECLARATIONDATA_INVALID_DOWNSHIFT_FACTORS = new ErrorCode(nameof(DECLARATIONDATA_INVALID_DOWNSHIFT_FACTORS));
        public static readonly ErrorCode DECLARATIONDATA_INSUFFICIENT_GEARS = new ErrorCode(nameof(DECLARATIONDATA_INSUFFICIENT_GEARS));
        public static readonly ErrorCode DECLARATIONDATA_INVALID_ITERATION = new ErrorCode(nameof(DECLARATIONDATA_INVALID_ITERATION));
        public static readonly ErrorCode DECLARATIONDATA_INVALID_FLOOR_TYPE = new ErrorCode(nameof(DECLARATIONDATA_INVALID_FLOOR_TYPE));
        public static readonly ErrorCode DECLARATIONDATA_INVALID_ICE_MODE = new ErrorCode(nameof(DECLARATIONDATA_INVALID_ICE_MODE));
        public static readonly ErrorCode DECLARATIONDATA_UNSUPPORTED_OPERATION = new ErrorCode(nameof(DECLARATIONDATA_UNSUPPORTED_OPERATION));

        public static readonly ErrorCode DELAUNAY_TRIANGLE_POINT_MISMATCH = new ErrorCode(nameof(DELAUNAY_TRIANGLE_POINT_MISMATCH));
        public static readonly ErrorCode DELAUNAY_DUPLICATE_POINTS = new ErrorCode(nameof(DELAUNAY_DUPLICATE_POINTS));
        public static readonly ErrorCode DELAUNAY_CANNOT_INTERPOLATE = new ErrorCode(nameof(DELAUNAY_CANNOT_INTERPOLATE));

        public static readonly ErrorCode DRIVER_DISTANCE_UNEXPECTEDLY_ADJUSTED = new ErrorCode(nameof(DRIVER_DISTANCE_UNEXPECTEDLY_ADJUSTED));
        public static readonly ErrorCode DRIVER_VELOCITY_NOT_ZERO_WHEN_HALTING = new ErrorCode(nameof(DRIVER_VELOCITY_NOT_ZERO_WHEN_HALTING));
        public static readonly ErrorCode DRIVER_LOOKAHEAD_DATA_MISSING = new ErrorCode(nameof(DRIVER_LOOKAHEAD_DATA_MISSING));
        public static readonly ErrorCode DRIVER_OPERATING_POINT_NOT_FOUND = new ErrorCode(nameof(DRIVER_OPERATING_POINT_NOT_FOUND));
        public static readonly ErrorCode DRIVER_TORQUE_CONVERTER_MISSING = new ErrorCode(nameof(DRIVER_TORQUE_CONVERTER_MISSING));
        public static readonly ErrorCode DRIVER_MISSING_DRY_RUN_RESPONSE = new ErrorCode(nameof(DRIVER_MISSING_DRY_RUN_RESPONSE));
        public static readonly ErrorCode DRIVER_MODE_INVALID_PHASE = new ErrorCode(nameof(DRIVER_MODE_INVALID_PHASE));

        public static readonly ErrorCode ELECTRICMOTOR_CANNOT_PROVIDE_TORQUE = new ErrorCode(nameof(ELECTRICMOTOR_CANNOT_PROVIDE_TORQUE));
        public static readonly ErrorCode ELECTRICMOTOR_EFFICIENCY_MAP_BAD_ENTRY = new ErrorCode(nameof(ELECTRICMOTOR_EFFICIENCY_MAP_BAD_ENTRY));
        public static readonly ErrorCode ELECTRICMOTOR_CANNOT_CREATE_VOLTAGE_LEVEL_DATA = new ErrorCode(nameof(ELECTRICMOTOR_CANNOT_CREATE_VOLTAGE_LEVEL_DATA));
        public static readonly ErrorCode ELECTRICMOTOR_INVALID_EFFICIENCY_MAP = new ErrorCode(nameof(ELECTRICMOTOR_INVALID_EFFICIENCY_MAP));
        public static readonly ErrorCode ELECTRICMOTOR_FAILED_TO_GET_MAX_RECUPERATION_TORQUE = new ErrorCode(nameof(ELECTRICMOTOR_FAILED_TO_GET_MAX_RECUPERATION_TORQUE));
        public static readonly ErrorCode ELECTRICMOTOR_BAD_TORQUE_LIMITATION = new ErrorCode(nameof(ELECTRICMOTOR_BAD_TORQUE_LIMITATION));
        public static readonly ErrorCode ELECTRICMOTOR_MULTIPLES_UNSUPPORTED = new ErrorCode(nameof(ELECTRICMOTOR_MULTIPLES_UNSUPPORTED));
        public static readonly ErrorCode ELECTRICMOTOR_IHPC_NO_GEARS_PROVIDED = new ErrorCode(nameof(ELECTRICMOTOR_IHPC_NO_GEARS_PROVIDED));
        public static readonly ErrorCode ELECTRICMOTOR_IHPC_GEARS_MISMATCH = new ErrorCode(nameof(ELECTRICMOTOR_IHPC_GEARS_MISMATCH));
        public static readonly ErrorCode ELECTRICMOTOR_INVALID_POSITION = new ErrorCode(nameof(ELECTRICMOTOR_INVALID_POSITION));
        public static readonly ErrorCode ELECTRICMOTOR_TORQUELIMIT_MISSING_VOLTAGE_LEVEL = new ErrorCode(nameof(ELECTRICMOTOR_TORQUELIMIT_MISSING_VOLTAGE_LEVEL));
        public static readonly ErrorCode ELECTRICMOTOR_TORQUELIMIT_INSUFFICIENT = new ErrorCode(nameof(ELECTRICMOTOR_TORQUELIMIT_INSUFFICIENT));
        public static readonly ErrorCode ELECTRICMOTOR_OVERLOAD_INVALID_DATA = new ErrorCode(nameof(ELECTRICMOTOR_OVERLOAD_INVALID_DATA));
        public static readonly ErrorCode ELECTRICMOTOR_OVERLOAD_NO_POWER = new ErrorCode(nameof(ELECTRICMOTOR_OVERLOAD_NO_POWER));
        public static readonly ErrorCode ELECTRICMOTOR_INVALID_ANGULAR_VELOCITY = new ErrorCode(nameof(ELECTRICMOTOR_INVALID_ANGULAR_VELOCITY));
        public static readonly ErrorCode ELECTRICMOTOR_SPEED_IS_ZERO = new ErrorCode(nameof(ELECTRICMOTOR_SPEED_IS_ZERO));
        public static readonly ErrorCode ELECTRICMOTOR_INVALID_RATED_SPEED = new ErrorCode(nameof(ELECTRICMOTOR_INVALID_RATED_SPEED));
        public static readonly ErrorCode ELECTRICMOTOR_RATED_SPEED_NOT_FOUND = new ErrorCode(nameof(ELECTRICMOTOR_RATED_SPEED_NOT_FOUND));
        public static readonly ErrorCode ELECTRICMOTOR_P25_MISSING_GEAR_RATIO = new ErrorCode(nameof(ELECTRICMOTOR_P25_MISSING_GEAR_RATIO));
        public static readonly ErrorCode ELECTRICMOTOR_INVALID_OPERATING_POINT = new ErrorCode(nameof(ELECTRICMOTOR_INVALID_OPERATING_POINT));
        public static readonly ErrorCode ELECTRICMOTOR_FULL_LOAD_CURVE_MISSING = new ErrorCode(nameof(ELECTRICMOTOR_FULL_LOAD_CURVE_MISSING));
        public static readonly ErrorCode ELECTRICMOTOR_DRAG_CURVE_MISSING = new ErrorCode(nameof(ELECTRICMOTOR_DRAG_CURVE_MISSING));
        public static readonly ErrorCode ELECTRICMOTOR_EFFICIENCY_MAP_MISSING = new ErrorCode(nameof(ELECTRICMOTOR_EFFICIENCY_MAP_MISSING));

        public static readonly ErrorCode ELECTRICSYSTEM_FUELCELL_ALREADY_CONNECTED = new ErrorCode(nameof(ELECTRICSYSTEM_FUELCELL_ALREADY_CONNECTED));
        public static readonly ErrorCode ELECTRICSYSTEM_BATTERY_ALREADY_CONNECTED = new ErrorCode(nameof(ELECTRICSYSTEM_BATTERY_ALREADY_CONNECTED));

        public static readonly ErrorCode EXCEPTION_CANNOT_INFER_ERROR_TYPE = new ErrorCode(nameof(EXCEPTION_CANNOT_INFER_ERROR_TYPE));

        public static readonly ErrorCode FILE_FAILED_TO_READ = new ErrorCode(nameof(FILE_FAILED_TO_READ));
        public static readonly ErrorCode FILE_INVALID_FORMAT = new ErrorCode(nameof(FILE_INVALID_FORMAT));

        public static readonly ErrorCode FUEL_MISSING_TANKSYSTEM = new ErrorCode(nameof(FUEL_MISSING_TANKSYSTEM));

        public static readonly ErrorCode FUELCELL_POWERMAP_GENERATION_FAILED = new ErrorCode(nameof(FUELCELL_POWERMAP_GENERATION_FAILED));
        public static readonly ErrorCode FUELCELL_SYSTEM_INVALID_STRING_COUNT = new ErrorCode(nameof(FUELCELL_SYSTEM_INVALID_STRING_COUNT));
        public static readonly ErrorCode FUELCELL_STRING_INSUFFICIENT_CELLS = new ErrorCode(nameof(FUELCELL_STRING_INSUFFICIENT_CELLS));
        public static readonly ErrorCode FUELCELL_STRING_EXCESSIVE_CELLS = new ErrorCode(nameof(FUELCELL_STRING_EXCESSIVE_CELLS));
        public static readonly ErrorCode FUELCELL_LIMITS_EXCEED_MASS_FLOW_MAP = new ErrorCode(nameof(FUELCELL_LIMITS_EXCEED_MASS_FLOW_MAP));
        public static readonly ErrorCode FUELCELL_MAX_STRINGS_EXCEEDED = new ErrorCode(nameof(FUELCELL_MAX_STRINGS_EXCEEDED));
        public static readonly ErrorCode FUELCELL_MAX_CELLS_PER_STRING_EXCEEDED = new ErrorCode(nameof(FUELCELL_MAX_CELLS_PER_STRING_EXCEEDED));
        public static readonly ErrorCode FUELCELL_STRINGS_MISSING = new ErrorCode(nameof(FUELCELL_STRINGS_MISSING));
        public static readonly ErrorCode FUELCELL_DISTANCE_EXCEEDS_MODEL_DATA = new ErrorCode(nameof(FUELCELL_DISTANCE_EXCEEDS_MODEL_DATA));
        public static readonly ErrorCode FUELCELL_POWER_EXCEEDS_MODEL_DATA = new ErrorCode(nameof(FUELCELL_POWER_EXCEEDS_MODEL_DATA));
        public static readonly ErrorCode FUELCELL_NEGATIVE_POWER = new ErrorCode(nameof(FUELCELL_NEGATIVE_POWER));
        public static readonly ErrorCode FUELCELL_INSUFFICIENT_POWER = new ErrorCode(nameof(FUELCELL_INSUFFICIENT_POWER));
        public static readonly ErrorCode FUELCELL_POWER_CALCULATION_FAILED = new ErrorCode(nameof(FUELCELL_POWER_CALCULATION_FAILED));
        public static readonly ErrorCode FUELCELL_SOC_VIOLATION = new ErrorCode(nameof(FUELCELL_SOC_VIOLATION));
        public static readonly ErrorCode FUELCELL_UNEXPECTED_RESPONSE = new ErrorCode(nameof(FUELCELL_UNEXPECTED_RESPONSE));
        public static readonly ErrorCode FUELCELL_CANNOT_COMPENSATE_BATTERY_DELTA_ENERGY = new ErrorCode(nameof(FUELCELL_CANNOT_COMPENSATE_BATTERY_DELTA_ENERGY));
        public static readonly ErrorCode FUELCELL_EXCESSIVE_POWER = new ErrorCode(nameof(FUELCELL_EXCESSIVE_POWER));
        public static readonly ErrorCode FUELCELL_WINDOW_SIZE_TOO_BIG = new ErrorCode(nameof(FUELCELL_WINDOW_SIZE_TOO_BIG));
        public static readonly ErrorCode FUELCELL_MODALDATA_OUT_OF_BOUNDS = new ErrorCode(nameof(FUELCELL_MODALDATA_OUT_OF_BOUNDS));
        public static readonly ErrorCode FUELCELL_MASS_FLOW_MAP_MISSING = new ErrorCode(nameof(FUELCELL_MASS_FLOW_MAP_MISSING));

        public static readonly ErrorCode GEARBOX_UNSUPPORTED_TYPE = new ErrorCode(nameof(GEARBOX_UNSUPPORTED_TYPE));
        public static readonly ErrorCode GEARBOX_NO_GEARS_DEFINED = new ErrorCode(nameof(GEARBOX_NO_GEARS_DEFINED));
        public static readonly ErrorCode GEARBOX_REQUESTED_GEAR_NOT_AVAILABLE = new ErrorCode(nameof(GEARBOX_REQUESTED_GEAR_NOT_AVAILABLE));
        public static readonly ErrorCode GEARBOX_INVALID_GEAR_DATA = new ErrorCode(nameof(GEARBOX_INVALID_GEAR_DATA));
        public static readonly ErrorCode GEARBOX_IEPC_NO_GEARS_ALLOWED = new ErrorCode(nameof(GEARBOX_IEPC_NO_GEARS_ALLOWED));
        public static readonly ErrorCode GEARBOX_NOT_ENOUGH_GEARS_DEFINED = new ErrorCode(nameof(GEARBOX_NOT_ENOUGH_GEARS_DEFINED));
        public static readonly ErrorCode GEARBOX_GEAR_MISSING_TORQUE_CONVERTER = new ErrorCode(nameof(GEARBOX_GEAR_MISSING_TORQUE_CONVERTER));
        public static readonly ErrorCode GEARBOX_CANNOT_DISABLE_GEARS = new ErrorCode(nameof(GEARBOX_CANNOT_DISABLE_GEARS));
        public static readonly ErrorCode GEARBOX_MISSING_ELECTRIC_MOTOR_DATA = new ErrorCode(nameof(GEARBOX_MISSING_ELECTRIC_MOTOR_DATA));
        public static readonly ErrorCode GEARBOX_MISSING_GEARSHIFT_PARAMETERS = new ErrorCode(nameof(GEARBOX_MISSING_GEARSHIFT_PARAMETERS));
        public static readonly ErrorCode GEARBOX_MISSING_AT_ECOROLL_RELEASE_LOCKUP_CLUTCH = new ErrorCode(nameof(GEARBOX_MISSING_AT_ECOROLL_RELEASE_LOCKUP_CLUTCH));
        public static readonly ErrorCode GEARBOX_MISSING_TORQUE_CONVERTER_DATA = new ErrorCode(nameof(GEARBOX_MISSING_TORQUE_CONVERTER_DATA));
        public static readonly ErrorCode GEARBOX_MISSING_LOSS_MAP = new ErrorCode(nameof(GEARBOX_MISSING_LOSS_MAP));
        public static readonly ErrorCode GEARBOX_LOSS_MAP_INTERPOLATION_FAILED = new ErrorCode(nameof(GEARBOX_LOSS_MAP_INTERPOLATION_FAILED));
        public static readonly ErrorCode GEARBOX_LOSS_MAP_EXTRAPOLATED = new ErrorCode(nameof(GEARBOX_LOSS_MAP_EXTRAPOLATED));

        public static readonly ErrorCode GENSET_INVALID_RESPONSE = new ErrorCode(nameof(GENSET_INVALID_RESPONSE));

        public static readonly ErrorCode HYBRIDCONTROLLER_ELECTRICMOTOR_ALREADY_REGISTERED = new ErrorCode(nameof(HYBRIDCONTROLLER_ELECTRICMOTOR_ALREADY_REGISTERED));
        public static readonly ErrorCode HYBRIDCONTROLLER_RETRY_COUNT_EXCEEDED = new ErrorCode(nameof(HYBRIDCONTROLLER_RETRY_COUNT_EXCEEDED));
        public static readonly ErrorCode HYBRIDCONTROLLER_GENSET_INVALID_OPERATING_POINT = new ErrorCode(nameof(HYBRIDCONTROLLER_GENSET_INVALID_OPERATING_POINT));

        public static readonly ErrorCode HYBRIDSTRATEGY_INVALID_MINSOC = new ErrorCode(nameof(HYBRIDSTRATEGY_INVALID_MINSOC));
        public static readonly ErrorCode HYBRIDSTRATEGY_MISSING_PARAMETERS = new ErrorCode(nameof(HYBRIDSTRATEGY_MISSING_PARAMETERS));
        public static readonly ErrorCode HYBRIDSTRATEGY_UNSUPPORTED_GEARBOX = new ErrorCode(nameof(HYBRIDSTRATEGY_UNSUPPORTED_GEARBOX));
        public static readonly ErrorCode HYBRIDSTRATEGY_GENSET_ICE_SPEED_NOT_FOUND = new ErrorCode(nameof(HYBRIDSTRATEGY_GENSET_ICE_SPEED_NOT_FOUND));

        public static readonly ErrorCode ICE_POSITIVE_FULL_DRAG_TORQUE_WITH_NEGATIVE_TORQUE_DEMAND = new ErrorCode(nameof(ICE_POSITIVE_FULL_DRAG_TORQUE_WITH_NEGATIVE_TORQUE_DEMAND));
        public static readonly ErrorCode ICE_NEGATIVE_DYNAMIC_FULL_LOAD_TORQUE_WITH_POSITIVE_TORQUE_DEMAND = new ErrorCode(nameof(ICE_NEGATIVE_DYNAMIC_FULL_LOAD_TORQUE_WITH_POSITIVE_TORQUE_DEMAND));
        public static readonly ErrorCode ICE_ANGULAR_VELOCITY_IS_NULL = new ErrorCode(nameof(ICE_ANGULAR_VELOCITY_IS_NULL));
        public static readonly ErrorCode ICE_ANGULAR_VELOCITY_EXCEEDS_FULL_LOAD_CURVE = new ErrorCode(nameof(ICE_ANGULAR_VELOCITY_EXCEEDS_FULL_LOAD_CURVE));
        public static readonly ErrorCode ICE_SPEED_TOO_LOW = new ErrorCode(nameof(ICE_SPEED_TOO_LOW));
        public static readonly ErrorCode ICE_INVALID_FUEL_TYPE = new ErrorCode(nameof(ICE_INVALID_FUEL_TYPE));
        public static readonly ErrorCode ICE_FAILED_TO_COMPUTE_N80H = new ErrorCode(nameof(ICE_FAILED_TO_COMPUTE_N80H));
        public static readonly ErrorCode ICE_FAILED_TO_COMPUTE_N95H = new ErrorCode(nameof(ICE_FAILED_TO_COMPUTE_N95H));
        public static readonly ErrorCode ICE_FAILED_TO_COMPUTE_NP99H = new ErrorCode(nameof(ICE_FAILED_TO_COMPUTE_NP99H));
        public static readonly ErrorCode ICE_FAILED_TO_COMPUTE_NP98H = new ErrorCode(nameof(ICE_FAILED_TO_COMPUTE_NP98H));
        public static readonly ErrorCode ICE_FUEL_CONSUMPTION_MAP_INTERPOLATION_FAILED = new ErrorCode(nameof(ICE_FUEL_CONSUMPTION_MAP_INTERPOLATION_FAILED));
        public static readonly ErrorCode ICE_WHR_MAP_INTERPOLATION_FAILED = new ErrorCode(nameof(ICE_WHR_MAP_INTERPOLATION_FAILED));
        public static readonly ErrorCode ICE_INTERVAL_IS_ZERO = new ErrorCode(nameof(ICE_INTERVAL_IS_ZERO));
        public static readonly ErrorCode ICE_PT1_CALCULATION_FAILED = new ErrorCode(nameof(ICE_PT1_CALCULATION_FAILED));
        public static readonly ErrorCode ICE_IDLE_CONTROLLER_INVALID_ANGULAR_VELOCITY = new ErrorCode(nameof(ICE_IDLE_CONTROLLER_INVALID_ANGULAR_VELOCITY));
        public static readonly ErrorCode ICE_IDLE_CONTROLLER_INVALID_TORQUE = new ErrorCode(nameof(ICE_IDLE_CONTROLLER_INVALID_TORQUE));
        public static readonly ErrorCode ICE_FULL_LOAD_CURVE_MISSING = new ErrorCode(nameof(ICE_FULL_LOAD_CURVE_MISSING));
        public static readonly ErrorCode ICE_FUEL_CONSUMPTION_MAP_MISSING = new ErrorCode(nameof(ICE_FUEL_CONSUMPTION_MAP_MISSING));

        public static readonly ErrorCode IEPC_EFFICIENCY_MAP_BAD_ENTRY = new ErrorCode(nameof(IEPC_EFFICIENCY_MAP_BAD_ENTRY));
        public static readonly ErrorCode IEPC_GEARS_MISMATCH = new ErrorCode(nameof(IEPC_GEARS_MISMATCH));
        public static readonly ErrorCode IEPC_FULL_LOAD_CURVE_MISSING = new ErrorCode(nameof(IEPC_FULL_LOAD_CURVE_MISSING));
        public static readonly ErrorCode IEPC_POWER_MAP_MISSING = new ErrorCode(nameof(IEPC_POWER_MAP_MISSING));
        public static readonly ErrorCode IEPC_DRAG_CURVE_MISSING = new ErrorCode(nameof(IEPC_DRAG_CURVE_MISSING));

        public static readonly ErrorCode IHPC_FULL_LOAD_CURVE_MISSING = new ErrorCode(nameof(IHPC_FULL_LOAD_CURVE_MISSING));
        public static readonly ErrorCode IHPC_DRAG_CURVE_MISSING = new ErrorCode(nameof(IHPC_DRAG_CURVE_MISSING));
        public static readonly ErrorCode IHPC_POWER_MAP_MISSING = new ErrorCode(nameof(IHPC_POWER_MAP_MISSING));

        public static readonly ErrorCode IMC_INVALID_AVAILABILITY = new ErrorCode(nameof(IMC_INVALID_AVAILABILITY));
        public static readonly ErrorCode IMC_NOT_ALLOWED = new ErrorCode(nameof(IMC_NOT_ALLOWED));

        public static readonly ErrorCode INPUT_INVALID_DATASOURCE = new ErrorCode(nameof(INPUT_INVALID_DATASOURCE));
        public static readonly ErrorCode INPUT_NON_MULTISTEP = new ErrorCode(nameof(INPUT_NON_MULTISTEP));
        public static readonly ErrorCode INPUT_NOT_XML = new ErrorCode(nameof(INPUT_NOT_XML));
        public static readonly ErrorCode INPUT_UNSUPPORTED_TYPE = new ErrorCode(nameof(INPUT_UNSUPPORTED_TYPE));

        public static readonly ErrorCode JSON_UNSUPPORTED_INPUT_DATA = new ErrorCode(nameof(JSON_UNSUPPORTED_INPUT_DATA));
        public static readonly ErrorCode JSON_INVALID_JOB_TYPE = new ErrorCode(nameof(JSON_INVALID_JOB_TYPE));
        public static readonly ErrorCode JSON_CANNOT_READ_AUX_TECHNOLOGY = new ErrorCode(nameof(JSON_CANNOT_READ_AUX_TECHNOLOGY));
        public static readonly ErrorCode JSON_UNSUPPORTED_VEHICLE_FILE_TYPE = new ErrorCode(nameof(JSON_UNSUPPORTED_VEHICLE_FILE_TYPE));
        public static readonly ErrorCode JSON_UNSUPPORTED_FILE_VERSION = new ErrorCode(nameof(JSON_UNSUPPORTED_FILE_VERSION));
        public static readonly ErrorCode JSON_UNDEFINED_AXLENUMBER = new ErrorCode(nameof(JSON_UNDEFINED_AXLENUMBER));
        public static readonly ErrorCode JSON_INVALID_ARCHITECTURE = new ErrorCode(nameof(JSON_INVALID_ARCHITECTURE));
        public static readonly ErrorCode JSON_UNSUPPORTED_ARCHITECTURE = new ErrorCode(nameof(JSON_UNSUPPORTED_ARCHITECTURE));
        public static readonly ErrorCode JSON_PTO_AND_EPTO_NOT_ALLOWED = new ErrorCode(nameof(JSON_PTO_AND_EPTO_NOT_ALLOWED));
        public static readonly ErrorCode JSON_ARRAY_EXPECTED = new ErrorCode(nameof(JSON_ARRAY_EXPECTED));
        public static readonly ErrorCode JSON_MISSING_FIELD = new ErrorCode(nameof(JSON_MISSING_FIELD));
        public static readonly ErrorCode JSON_MISSING_DATA = new ErrorCode(nameof(JSON_MISSING_DATA));

        public static readonly ErrorCode MATH_LOWER_BOUND_GREATER_THAN_UPPER_BOUND = new ErrorCode(nameof(MATH_LOWER_BOUND_GREATER_THAN_UPPER_BOUND));
        public static readonly ErrorCode MATH_DISTANCE_NOT_POSITIVE = new ErrorCode(nameof(MATH_DISTANCE_NOT_POSITIVE));
        public static readonly ErrorCode MATH_SPEED_NOT_POSITIVE_WHEN_ACCELERATING = new ErrorCode(nameof(MATH_SPEED_NOT_POSITIVE_WHEN_ACCELERATING));
        public static readonly ErrorCode MATH_NO_SOLUTION_FOR_TIME_INTERVAL = new ErrorCode(nameof(MATH_NO_SOLUTION_FOR_TIME_INTERVAL));
        public static readonly ErrorCode MATH_TRIANGLE_NOT_EXTRAPOLATABLE_BY_PLANE = new ErrorCode(nameof(MATH_TRIANGLE_NOT_EXTRAPOLATABLE_BY_PLANE));

        public static readonly ErrorCode MODALDATA_FUEL_ALREADY_ADDED = new ErrorCode(nameof(MODALDATA_FUEL_ALREADY_ADDED));
        public static readonly ErrorCode MODALDATA_UNSUPPORTED_INDEX = new ErrorCode(nameof(MODALDATA_UNSUPPORTED_INDEX));
        public static readonly ErrorCode MODALDATA_COLUMN_NOT_FOUND = new ErrorCode(nameof(MODALDATA_COLUMN_NOT_FOUND));
        public static readonly ErrorCode MODALDATA_INVALID_FUEL = new ErrorCode(nameof(MODALDATA_INVALID_FUEL));

        public static readonly ErrorCode MRF_HASH_DIFFERS_FROM_CALCULATED_HASH = new ErrorCode(nameof(MRF_HASH_DIFFERS_FROM_CALCULATED_HASH));

        public static readonly ErrorCode ONEHERZFILTER_NEGATIVE_DISTANCE = new ErrorCode(nameof(ONEHERZFILTER_NEGATIVE_DISTANCE));

        public static readonly ErrorCode PREPROCESSING_MISSING_DATA = new ErrorCode(nameof(PREPROCESSING_MISSING_DATA));

        public static readonly ErrorCode PTO_MISSING_DATA = new ErrorCode(nameof(PTO_MISSING_DATA));
        public static readonly ErrorCode PTO_ACTIVATION_REQUESTED_WHILE_PTO_ACTIVE = new ErrorCode(nameof(PTO_ACTIVATION_REQUESTED_WHILE_PTO_ACTIVE));
        public static readonly ErrorCode PTO_MISSING_GEARBOX = new ErrorCode(nameof(PTO_MISSING_GEARBOX));
        public static readonly ErrorCode PTO_NON_IDLE_REQUEST = new ErrorCode(nameof(PTO_NON_IDLE_REQUEST));

        public static readonly ErrorCode REESS_EMPTY_BEFORE_CYCLE_END = new ErrorCode(nameof(REESS_EMPTY_BEFORE_CYCLE_END));
        public static readonly ErrorCode REESS_INTERNAL_RESISTANCE_LOOKUP_FAILED = new ErrorCode(nameof(REESS_INTERNAL_RESISTANCE_LOOKUP_FAILED));
        public static readonly ErrorCode REESS_MULTIPLE_TYPES = new ErrorCode(nameof(REESS_MULTIPLE_TYPES));
        public static readonly ErrorCode REESS_BATTERY_MISSING = new ErrorCode(nameof(REESS_BATTERY_MISSING));
        public static readonly ErrorCode REESS_SUPERCAP_NOT_ALLOWED = new ErrorCode(nameof(REESS_SUPERCAP_NOT_ALLOWED));
        public static readonly ErrorCode REESS_MINSOC_NOT_LESS_THAN_MAXSOC = new ErrorCode(nameof(REESS_MINSOC_NOT_LESS_THAN_MAXSOC));
        public static readonly ErrorCode REESS_INVALID_DATASOURCE_TYPE = new ErrorCode(nameof(REESS_INVALID_DATASOURCE_TYPE));
        public static readonly ErrorCode REESS_MISSING = new ErrorCode(nameof(REESS_MISSING));
        public static readonly ErrorCode REESS_INITIAL_SOC_OUT_OF_BOUNDS = new ErrorCode(nameof(REESS_INITIAL_SOC_OUT_OF_BOUNDS));
        public static readonly ErrorCode REESS_INVALID_SOC = new ErrorCode(nameof(REESS_INVALID_SOC));
        public static readonly ErrorCode REESS_SUPERCAP_MISSING = new ErrorCode(nameof(REESS_SUPERCAP_MISSING));
        public static readonly ErrorCode REESS_RI_CURVE_MISSING = new ErrorCode(nameof(REESS_RI_CURVE_MISSING));
        public static readonly ErrorCode REESS_SOC_CURVE_MISSING = new ErrorCode(nameof(REESS_SOC_CURVE_MISSING));
        public static readonly ErrorCode REESS_MAX_CURRENT_MAP_MISSING = new ErrorCode(nameof(REESS_MAX_CURRENT_MAP_MISSING));

        public static readonly ErrorCode REPORT_NO_RESULT_WRITER = new ErrorCode(nameof(REPORT_NO_RESULT_WRITER));
        public static readonly ErrorCode REPORT_NO_MATCHING_RESULT = new ErrorCode(nameof(REPORT_NO_MATCHING_RESULT));
        public static readonly ErrorCode REPORT_RESULT_OVC_INVALID_MODE = new ErrorCode(nameof(REPORT_RESULT_OVC_INVALID_MODE));
        public static readonly ErrorCode REPORT_RESULT_OVC_MISMATCH = new ErrorCode(nameof(REPORT_RESULT_OVC_MISMATCH));
        public static readonly ErrorCode REPORT_RESULT_INVALID_VEHICLE_CLASS = new ErrorCode(nameof(REPORT_RESULT_INVALID_VEHICLE_CLASS));
        public static readonly ErrorCode REPORT_RESULT_VTP_NOT_FOUND = new ErrorCode(nameof(REPORT_RESULT_VTP_NOT_FOUND));
        public static readonly ErrorCode REPORT_RESULT_INVALID_WEIGHTING_FACTORS = new ErrorCode(nameof(REPORT_RESULT_INVALID_WEIGHTING_FACTORS));
        public static readonly ErrorCode REPORT_PRIMARY_RESULT_NOT_FOUND = new ErrorCode(nameof(REPORT_PRIMARY_RESULT_NOT_FOUND));
        public static readonly ErrorCode REPORT_INVALID_INPUT = new ErrorCode(nameof(REPORT_INVALID_INPUT));

        public static readonly ErrorCode RESOURCE_NOT_FOUND = new ErrorCode(nameof(RESOURCE_NOT_FOUND));

        public static readonly ErrorCode RETARDER_BAD_DATA = new ErrorCode(nameof(RETARDER_BAD_DATA));
        public static readonly ErrorCode RETARDER_LOSS_MAP_EXTRAPOLATED = new ErrorCode(nameof(RETARDER_LOSS_MAP_EXTRAPOLATED));

        public static readonly ErrorCode SEARCH_FAILED = new ErrorCode(nameof(SEARCH_FAILED));
        public static readonly ErrorCode SEARCH_ABORTED = new ErrorCode(nameof(SEARCH_ABORTED));

        public static readonly ErrorCode SHIFTLINESSET_NO_LINES_FOUND = new ErrorCode(nameof(SHIFTLINESSET_NO_LINES_FOUND));

        public static readonly ErrorCode SHIFTSTRATEGY_INVALID_GEAR = new ErrorCode(nameof(SHIFTSTRATEGY_INVALID_GEAR));
        public static readonly ErrorCode SHIFTSTRATEGY_HIGHER_GEAR_NOT_AVAILABLE = new ErrorCode(nameof(SHIFTSTRATEGY_HIGHER_GEAR_NOT_AVAILABLE));
        public static readonly ErrorCode SHIFTSTRATEGY_LOWER_GEAR_NOT_AVAILABLE = new ErrorCode(nameof(SHIFTSTRATEGY_LOWER_GEAR_NOT_AVAILABLE));
        public static readonly ErrorCode SHIFTSTRATEGY_MISSING_PARAMETERS = new ErrorCode(nameof(SHIFTSTRATEGY_MISSING_PARAMETERS));
        public static readonly ErrorCode SHIFTSTRATEGY_UNSUPPORTED_GEARBOX = new ErrorCode(nameof(SHIFTSTRATEGY_UNSUPPORTED_GEARBOX));
        public static readonly ErrorCode SHIFTSTRATEGY_INVALID_LOADSTAGE_THRESHOLDS = new ErrorCode(nameof(SHIFTSTRATEGY_INVALID_LOADSTAGE_THRESHOLDS));
        public static readonly ErrorCode SHIFTSTRATEGY_INVALID_SHIFTSPEEDS_TC2LOCKED = new ErrorCode(nameof(SHIFTSTRATEGY_INVALID_SHIFTSPEEDS_TC2LOCKED));
        public static readonly ErrorCode SHIFTSTRATEGY_SKIPPED_GEAR_TC2LOCKED = new ErrorCode(nameof(SHIFTSTRATEGY_SKIPPED_GEAR_TC2LOCKED));
        public static readonly ErrorCode SHIFTSTRATEGY_INVALID_ELECTRICMOTOR_POSITION = new ErrorCode(nameof(SHIFTSTRATEGY_INVALID_ELECTRICMOTOR_POSITION));
        public static readonly ErrorCode SHIFTSTRATEGY_MISSING_ELECTRICMOTOR_DATA = new ErrorCode(nameof(SHIFTSTRATEGY_MISSING_ELECTRICMOTOR_DATA));
        public static readonly ErrorCode SHIFTSTRATEGY_UNSUPPORTED_VEHICLE = new ErrorCode(nameof(SHIFTSTRATEGY_UNSUPPORTED_VEHICLE));
        public static readonly ErrorCode SHIFTSTRATEGY_INVALID_ELECTRICMOTOR_DATA = new ErrorCode(nameof(SHIFTSTRATEGY_INVALID_ELECTRICMOTOR_DATA));

        public static readonly ErrorCode SI_NAN_NOT_ALLOWED = new ErrorCode(nameof(SI_NAN_NOT_ALLOWED));
        public static readonly ErrorCode SI_INFINITY_NOT_ALLOWED = new ErrorCode(nameof(SI_INFINITY_NOT_ALLOWED));
        public static readonly ErrorCode SI_UNIT_CONVERSION_FAILED = new ErrorCode(nameof(SI_UNIT_CONVERSION_FAILED));
        public static readonly ErrorCode SI_OPERANDS_HAVE_DIFFERENT_UNITS = new ErrorCode(nameof(SI_OPERANDS_HAVE_DIFFERENT_UNITS));
        public static readonly ErrorCode SI_DIVIDE_BY_ZERO = new ErrorCode(nameof(SI_DIVIDE_BY_ZERO));
        public static readonly ErrorCode SI_TOLERANCE_HAS_DIFFERENT_UNIT = new ErrorCode(nameof(SI_TOLERANCE_HAS_DIFFERENT_UNIT));

        public static readonly ErrorCode SIMFACTORY_FOLLOWUP_PRIMARY_FAILED = new ErrorCode(nameof(SIMFACTORY_FOLLOWUP_PRIMARY_FAILED));
        public static readonly ErrorCode SIMFACTORY_COMPLETED_BAD_ENTRIES = new ErrorCode(nameof(SIMFACTORY_COMPLETED_BAD_ENTRIES));
        public static readonly ErrorCode SIMFACTORY_INVALID_CYCLE = new ErrorCode(nameof(SIMFACTORY_INVALID_CYCLE));

        public static readonly ErrorCode SPEEDPREPROCESSOR_ROLLING_CANNOT_FIND_ACCELERATION = new ErrorCode(nameof(SPEEDPREPROCESSOR_ROLLING_CANNOT_FIND_ACCELERATION));

        public static readonly ErrorCode SUMMARY_INVALID_JOB_NUMBER = new ErrorCode(nameof(SUMMARY_INVALID_JOB_NUMBER));

        public static readonly ErrorCode SYSTEM_INVALID_OPERATION = new ErrorCode(nameof(SYSTEM_INVALID_OPERATION));
        public static readonly ErrorCode SYSTEM_KEY_NOT_FOUND = new ErrorCode(nameof(SYSTEM_KEY_NOT_FOUND));
        public static readonly ErrorCode SYSTEM_DUPLICATE_KEY = new ErrorCode(nameof(SYSTEM_DUPLICATE_KEY));
        public static readonly ErrorCode SYSTEM_INDEX_OUT_OF_RANGE = new ErrorCode(nameof(SYSTEM_INDEX_OUT_OF_RANGE));
        public static readonly ErrorCode SYSTEM_NULL_REFERENCE = new ErrorCode(nameof(SYSTEM_NULL_REFERENCE));
        public static readonly ErrorCode SYSTEM_FORMAT = new ErrorCode(nameof(SYSTEM_FORMAT));
        public static readonly ErrorCode SYSTEM_OVERFLOW = new ErrorCode(nameof(SYSTEM_OVERFLOW));
        public static readonly ErrorCode SYSTEM_ARGUMENT_NULL = new ErrorCode(nameof(SYSTEM_ARGUMENT_NULL));
        public static readonly ErrorCode SYSTEM_NO_EXACT_MATCH = new ErrorCode(nameof(SYSTEM_NO_EXACT_MATCH));

        public static readonly ErrorCode TORQUECONVERTER_NO_OPERATING_POINT_FOUND = new ErrorCode(nameof(TORQUECONVERTER_NO_OPERATING_POINT_FOUND));
        public static readonly ErrorCode TORQUECONVERTER_INVALID_OPERATING_POINT = new ErrorCode(nameof(TORQUECONVERTER_INVALID_OPERATING_POINT));
        public static readonly ErrorCode TORQUECONVERTER_GEAR_INVALID_DATA = new ErrorCode(nameof(TORQUECONVERTER_GEAR_INVALID_DATA));
        public static readonly ErrorCode TORQUECONVERTER_RANGE_INSUFFICIENT = new ErrorCode(nameof(TORQUECONVERTER_RANGE_INSUFFICIENT));
        public static readonly ErrorCode TORQUECONVERTER_NO_SOLUTION_FOUND = new ErrorCode(nameof(TORQUECONVERTER_NO_SOLUTION_FOUND));

        public static readonly ErrorCode TORQUESPLITTER_NO_RATIOS_FOUND = new ErrorCode(nameof(TORQUESPLITTER_NO_RATIOS_FOUND));
        public static readonly ErrorCode TORQUESPLITTER_ELECTRICSYSTEM_NOT_READY = new ErrorCode(nameof(TORQUESPLITTER_ELECTRICSYSTEM_NOT_READY));

        public static readonly ErrorCode VECTORUN_CYCLE_CANNOT_FINISH = new ErrorCode(nameof(VECTORUN_CYCLE_CANNOT_FINISH));
        public static readonly ErrorCode VECTORUN_DISTANCE_EXCEEDED = new ErrorCode(nameof(VECTORUN_DISTANCE_EXCEEDED));
        public static readonly ErrorCode VECTORUN_MAXIMUM_ITERATION_COUNT_EXCEEDED = new ErrorCode(nameof(VECTORUN_MAXIMUM_ITERATION_COUNT_EXCEEDED));
        public static readonly ErrorCode VECTORUN_UNEXPECTED_RESPONSE = new ErrorCode(nameof(VECTORUN_UNEXPECTED_RESPONSE));
        public static readonly ErrorCode VECTORUN_ABORTED = new ErrorCode(nameof(VECTORUN_ABORTED));
        public static readonly ErrorCode VECTORUN_IGNORED = new ErrorCode(nameof(VECTORUN_IGNORED));
        public static readonly ErrorCode VECTORUN_PREVIOUS_REQUEST_FAILED = new ErrorCode(nameof(VECTORUN_PREVIOUS_REQUEST_FAILED));
        public static readonly ErrorCode VECTORUN_INITIALIZATION_FAILED = new ErrorCode(nameof(VECTORUN_INITIALIZATION_FAILED));

        public static readonly ErrorCode VECTORUNDATAFACTORY_UNKNOWN_INPUT = new ErrorCode(nameof(VECTORUNDATAFACTORY_UNKNOWN_INPUT));
        public static readonly ErrorCode VECTORUNDATAFACTORY_UNSUPPORTED_VEHICLE_TYPE = new ErrorCode(nameof(VECTORUNDATAFACTORY_UNSUPPORTED_VEHICLE_TYPE));
        public static readonly ErrorCode VECTORUNDATAFACTORY_NO_RESULTS_IN_PRIMARY_VEHICLE_REPORT = new ErrorCode(nameof(VECTORUNDATAFACTORY_NO_RESULTS_IN_PRIMARY_VEHICLE_REPORT));
        public static readonly ErrorCode VECTORUNDATAFACTORY_PRIMARY_RUN_IGNORED = new ErrorCode(nameof(VECTORUNDATAFACTORY_PRIMARY_RUN_IGNORED));
        public static readonly ErrorCode VECTORUNDATAFACTORY_PRIMARY_RUN_FAILED = new ErrorCode(nameof(VECTORUNDATAFACTORY_PRIMARY_RUN_FAILED));
        public static readonly ErrorCode VECTORUNDATAFACTORY_SEGMENT_NOT_FOUND = new ErrorCode(nameof(VECTORUNDATAFACTORY_SEGMENT_NOT_FOUND));
        public static readonly ErrorCode VECTORUNDATAFACTORY_MISSION_NOT_FOUND = new ErrorCode(nameof(VECTORUNDATAFACTORY_MISSION_NOT_FOUND));
        public static readonly ErrorCode VECTORUNDATAFACTORY_UNSPECIFIED_ENGINE_MODE = new ErrorCode(nameof(VECTORUNDATAFACTORY_UNSPECIFIED_ENGINE_MODE));
        public static readonly ErrorCode VECTORUNDATAFACTORY_BAD_REPORT_TYPE = new ErrorCode(nameof(VECTORUNDATAFACTORY_BAD_REPORT_TYPE));

        public static readonly ErrorCode VEHICLE_MISSING_AXLEGEAR = new ErrorCode(nameof(VEHICLE_MISSING_AXLEGEAR));
        public static readonly ErrorCode VEHICLE_UNSUPPORTED_LEGISLATIVE_CLASS = new ErrorCode(nameof(VEHICLE_UNSUPPORTED_LEGISLATIVE_CLASS));
        public static readonly ErrorCode VEHICLE_INCOMPATIBLE_AUXILIARY_CONFIGURATION = new ErrorCode(nameof(VEHICLE_INCOMPATIBLE_AUXILIARY_CONFIGURATION));
        public static readonly ErrorCode VEHICLE_HYBRID_NO_ELECTRIC_MOTOR = new ErrorCode(nameof(VEHICLE_HYBRID_NO_ELECTRIC_MOTOR));
        public static readonly ErrorCode VEHICLE_INSUFFICIENT_AXLES = new ErrorCode(nameof(VEHICLE_INSUFFICIENT_AXLES));
        public static readonly ErrorCode VEHICLE_MISSING_PARAMETER = new ErrorCode(nameof(VEHICLE_MISSING_PARAMETER));
        public static readonly ErrorCode VEHICLE_TOTAL_MASS_EXCEEDS_GROSS_VEHICLE_MASS = new ErrorCode(nameof(VEHICLE_TOTAL_MASS_EXCEEDS_GROSS_VEHICLE_MASS));
        public static readonly ErrorCode VEHICLE_INVALID_LOADING_TYPE = new ErrorCode(nameof(VEHICLE_INVALID_LOADING_TYPE));
        public static readonly ErrorCode VEHICLE_MISSING_ICE = new ErrorCode(nameof(VEHICLE_MISSING_ICE));
        public static readonly ErrorCode VEHICLE_MULTIPLE_POWERTRAINS = new ErrorCode(nameof(VEHICLE_MULTIPLE_POWERTRAINS));
        public static readonly ErrorCode VEHICLE_PTO_IN_BUS_NOT_ALLOWED = new ErrorCode(nameof(VEHICLE_PTO_IN_BUS_NOT_ALLOWED));
        public static readonly ErrorCode VEHICLE_ARCHITECTURE_MISMATCH = new ErrorCode(nameof(VEHICLE_ARCHITECTURE_MISMATCH));
        public static readonly ErrorCode VEHICLE_DUPLICATE_AXLE_NUMBER = new ErrorCode(nameof(VEHICLE_DUPLICATE_AXLE_NUMBER));
        public static readonly ErrorCode VEHICLE_INVALID_AXLE_NUMBER = new ErrorCode(nameof(VEHICLE_INVALID_AXLE_NUMBER));
        public static readonly ErrorCode VEHICLE_ELECTRICMOTOR_MISMATCH = new ErrorCode(nameof(VEHICLE_ELECTRICMOTOR_MISMATCH));
        public static readonly ErrorCode VEHICLE_INVALID_GEARBOX = new ErrorCode(nameof(VEHICLE_INVALID_GEARBOX));
        public static readonly ErrorCode VEHICLE_CANNOT_SUPPORT_GENSET = new ErrorCode(nameof(VEHICLE_CANNOT_SUPPORT_GENSET));
        public static readonly ErrorCode VEHICLE_GENSET_MISMATCH = new ErrorCode(nameof(VEHICLE_GENSET_MISMATCH));
        public static readonly ErrorCode VEHICLE_UNSUPPORTED_HYBRID_P0 = new ErrorCode(nameof(VEHICLE_UNSUPPORTED_HYBRID_P0));
        public static readonly ErrorCode VEHICLE_INVALID_ELECTRICMOTOR_POSITION = new ErrorCode(nameof(VEHICLE_INVALID_ELECTRICMOTOR_POSITION));
        public static readonly ErrorCode VEHICLE_BUS_AUXILIARIES_MISSING = new ErrorCode(nameof(VEHICLE_BUS_AUXILIARIES_MISSING));
        public static readonly ErrorCode VEHICLE_BUS_AUXILIARIES_DISCONNECTED_FROM_REESS = new ErrorCode(nameof(VEHICLE_BUS_AUXILIARIES_DISCONNECTED_FROM_REESS));
        public static readonly ErrorCode VEHICLE_MISSING_GEARBOX = new ErrorCode(nameof(VEHICLE_MISSING_GEARBOX));
        public static readonly ErrorCode VEHICLE_INVALID_CYCLE = new ErrorCode(nameof(VEHICLE_INVALID_CYCLE));
        public static readonly ErrorCode VEHICLE_MISSING_PTO_CYCLE = new ErrorCode(nameof(VEHICLE_MISSING_PTO_CYCLE));
        public static readonly ErrorCode VEHICLE_TOTAL_WEIGHT_IS_ZERO = new ErrorCode(nameof(VEHICLE_TOTAL_WEIGHT_IS_ZERO));
        public static readonly ErrorCode VEHICLE_DYNAMIC_TYRE_RADIUS_NOT_SET = new ErrorCode(nameof(VEHICLE_DYNAMIC_TYRE_RADIUS_NOT_SET));
        public static readonly ErrorCode VEHICLE_ANGULAR_VELOCITY_EXCEEDS_MAX_TORQUE_CURVE = new ErrorCode(nameof(VEHICLE_ANGULAR_VELOCITY_EXCEEDS_MAX_TORQUE_CURVE));
        public static readonly ErrorCode VEHICLE_MISSING_TORQUE_CONVERTER = new ErrorCode(nameof(VEHICLE_MISSING_TORQUE_CONVERTER));
        public static readonly ErrorCode VEHICLE_MISSING_BRAKES = new ErrorCode(nameof(VEHICLE_MISSING_BRAKES));
        public static readonly ErrorCode VEHICLE_MISSING = new ErrorCode(nameof(VEHICLE_MISSING));
        public static readonly ErrorCode VEHICLE_MISSING_ELECTRICMOTOR = new ErrorCode(nameof(VEHICLE_MISSING_ELECTRICMOTOR));
        public static readonly ErrorCode VEHICLE_AIRDRAG_MODIFIED_NOT_SET = new ErrorCode(nameof(VEHICLE_AIRDRAG_MODIFIED_NOT_SET));
        public static readonly ErrorCode VEHICLE_MISSING_SLEEPERCAB = new ErrorCode(nameof(VEHICLE_MISSING_SLEEPERCAB));
        public static readonly ErrorCode VEHICLE_EXEMPTED = new ErrorCode(nameof(VEHICLE_EXEMPTED));
        public static readonly ErrorCode VEHICLE_UNSUPPORTED_TYPE = new ErrorCode(nameof(VEHICLE_UNSUPPORTED_TYPE));

        public static readonly ErrorCode VTP_CYCLE_NOT_FOUND = new ErrorCode(nameof(VTP_CYCLE_NOT_FOUND));
        public static readonly ErrorCode VTP_CYCLE_INVALID = new ErrorCode(nameof(VTP_CYCLE_INVALID));
        public static readonly ErrorCode VTP_VECTO_VERSION_TOO_OLD = new ErrorCode(nameof(VTP_VECTO_VERSION_TOO_OLD));

        public static readonly ErrorCode WHEELEND_FRICTION_EXCEEDS_STANDARD = new ErrorCode(nameof(WHEELEND_FRICTION_EXCEEDS_STANDARD));
        public static readonly ErrorCode WHEELEND_FRICTION_NEGATIVE = new ErrorCode(nameof(WHEELEND_FRICTION_NEGATIVE));
        public static readonly ErrorCode WHEELEND_INVALID_AXLE_TYPE = new ErrorCode(nameof(WHEELEND_INVALID_AXLE_TYPE));

        public static readonly ErrorCode WHR_POWER_MAP_BAD_ENTRY = new ErrorCode(nameof(WHR_POWER_MAP_BAD_ENTRY));
        public static readonly ErrorCode WHR_MISSING_DATA = new ErrorCode(nameof(WHR_MISSING_DATA));

        public static readonly ErrorCode XML_FAILED_TO_CREATE_COMPONENT = new ErrorCode(nameof(XML_FAILED_TO_CREATE_COMPONENT));
        public static readonly ErrorCode XML_FAILED_TO_READ_FILE = new ErrorCode(nameof(XML_FAILED_TO_READ_FILE));
        public static readonly ErrorCode XML_UNSUPPORTED_VERSION = new ErrorCode(nameof(XML_UNSUPPORTED_VERSION));
        public static readonly ErrorCode XML_UNSUPPORTED_DOCUMENT_TYPE = new ErrorCode(nameof(XML_UNSUPPORTED_DOCUMENT_TYPE));
        public static readonly ErrorCode XML_MISSING_ATTRIBUTE = new ErrorCode(nameof(XML_MISSING_ATTRIBUTE));
        public static readonly ErrorCode XML_MISSING_DATA = new ErrorCode(nameof(XML_MISSING_DATA));
        public static readonly ErrorCode XML_MISSING_NODE = new ErrorCode(nameof(XML_MISSING_NODE));
        public static readonly ErrorCode XML_DUPLICATE_NODE = new ErrorCode(nameof(XML_DUPLICATE_NODE));
        public static readonly ErrorCode XML_NULL_ARGUMENT = new ErrorCode(nameof(XML_NULL_ARGUMENT));
        public static readonly ErrorCode XML_EMPTY_DOCUMENT = new ErrorCode(nameof(XML_EMPTY_DOCUMENT));
        public static readonly ErrorCode XML_BAD_DATA = new ErrorCode(nameof(XML_BAD_DATA));
        public static readonly ErrorCode XML_VALIDATION_FAILED = new ErrorCode(nameof(XML_VALIDATION_FAILED));
    }

    [Serializable]
    public class ErrorCode : IEquatable<ErrorCode>
    {
        public ErrorCode(string code)
        {
            Debug.Assert(!string.IsNullOrEmpty(code));

            Code = code;
        }

        private ErrorCode() { }

        public string Code { get; }

        public override string ToString()
        {
            return Code;
        }

        public override bool Equals(object obj) => Equals(obj as ErrorCode);

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

        public bool Equals(ErrorCode other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(other, null))
            {
                return false;
            }

            return Code == other.Code;
        }

        public static bool operator ==(ErrorCode a, ErrorCode b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(ErrorCode a, ErrorCode b) => !(a == b);
    }
}
