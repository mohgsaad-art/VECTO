
<xsl:transform version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
	<!--
    This XSL transformation is intended to be applied as first canonicalization step when computing the hash
    for VECTO component data or VECTO job data.

    The transformation performs the following operations:
      - strip off namespace prefixes
         (although namespace prefixes are considered part of the signature for the purpose of hashing VECTO data
         it does not provide additional semantics because the file has to validate against a XSD schema anyways 
         and may cause troubles when re-creating the VECTO data from database systems)
	  - ignore xsi:type attributes
      - normalize the whitespaces of all attribute values and text nodes
         leading and trailing whitespaces are removed
         multiple whitespaces are replaced by a single whitespace
      - sort entries of FuelConsumptionMap and loss-maps (i.e, transmission, axlegear, angledrive)
	  - sort entries of FullLoadAndDragCurve	
	  - sort entries of TorqueLossMap
      - sort entries of RetarderLossMap
	  - sort entries of TorqueLimits
	  - sort Gears 
      - sort entries of Characteristics
      - sort Axles 
	  - sort entries of MaxTorqueCurve
	  - sort entries of DragCurve
	  - sort entries of Conditioning
	  - sort entries of PowerMap
	  - sort VoltageLevels by Voltage value
	  - sort DragCurve by gear attribute value
	  - sort Fuel by type attribute value
	  - sort Mode by numbers of child Fuels
      - sort entries of ApplicableVehicleGroups 
	  - sort entries of OCV
      - sort entries of InternalResistance
	  - sort entries of CurrentLimits
-->	<xsl:output omit-xml-declaration="no" indent="yes"/>
	<xsl:template match="*">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*|node()"/>
		</xsl:element>
	</xsl:template>
	<xsl:template match="@xsi:type"/>
	<xsl:template match="@*">
		<xsl:attribute name="{name()}"><xsl:value-of select="normalize-space(.)"/></xsl:attribute>
	</xsl:template>
	<xsl:template match="text()">
         <xsl:value-of select="normalize-space(.)"/>
    </xsl:template>
	<xsl:template match="*[local-name()='FuelConsumptionMap']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@engineSpeed" order="ascending"/>
				<xsl:sort data-type="number" select="@torque" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='FuelCellPowerOutputConsumptionMap']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@powerOutput" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='FullLoadAndDragCurve']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@engineSpeed" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='TorqueLossMap']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@inputSpeed" order="ascending"/>
				<xsl:sort data-type="number" select="@inputTorque" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='RetarderLossMap']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@retarderSpeed" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='TorqueLimits']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@gear" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='Gears']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@number" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='Characteristics']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@speedRatio" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='Axles']">
		<xsl:apply-templates select="@*"/>
		<xsl:element name="{local-name()}">
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@axleNumber" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>

	<xsl:template match="*[local-name()='GearshiftCount' and @axleNumber]">
		<xsl:if test="count(preceding-sibling::*[local-name()='GearshiftCount']) = 0">
			<xsl:for-each select="../*[local-name()='GearshiftCount']">
				<xsl:sort data-type="number" select="@axleNumber" order="ascending"/>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="*"/>
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>

	<xsl:template match="*[local-name()='AverageGearboxEfficiency' and @axleNumber]">
		<xsl:if test="count(preceding-sibling::*[local-name()='AverageGearboxEfficiency']) = 0">
			<xsl:for-each select="../*[local-name()='AverageGearboxEfficiency']">
				<xsl:sort data-type="number" select="@axleNumber" order="ascending"/>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="*"/>
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>

	<xsl:template match="*[local-name()='AverageAxlegearEfficiency' and @axleNumber]">
		<xsl:if test="count(preceding-sibling::*[local-name()='AverageAxlegearEfficiency']) = 0">
			<xsl:for-each select="../*[local-name()='AverageAxlegearEfficiency']">
				<xsl:sort data-type="number" select="@axleNumber" order="ascending"/>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="*"/>
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>

	<xsl:template match="*[local-name()='PEVArchitecture' and @axleNumber]">
		<xsl:if test="count(preceding-sibling::*[local-name()='PEVArchitecture']) = 0">
			<xsl:for-each select="../*[local-name()='PEVArchitecture']">
				<xsl:sort data-type="number" select="@axleNumber" order="ascending"/>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="*"/>
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>

	<xsl:template match="*[local-name()='HEVArchitecture' and @axleNumber]">
		<xsl:if test="count(preceding-sibling::*[local-name()='HEVArchitecture']) = 0">
			<xsl:for-each select="../*[local-name()='HEVArchitecture']">
				<xsl:sort data-type="number" select="@axleNumber" order="ascending"/>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="*"/>
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>

	<xsl:template match="*[local-name()='FCHVArchitecture' and @axleNumber]">
		<xsl:if test="count(preceding-sibling::*[local-name()='FCHVArchitecture']) = 0">
			<xsl:for-each select="../*[local-name()='FCHVArchitecture']">
				<xsl:sort data-type="number" select="@axleNumber" order="ascending"/>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="*"/>
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>
	
	<xsl:template match="*[local-name()='MaxTorqueCurve']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@outShaftSpeed" order="ascending"/>
				<xsl:sort data-type="number" select="@maxTorque" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template name="DragCurveTemplate" match="*[local-name()='DragCurve']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@outShaftSpeed" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>		
	<xsl:template match="*[local-name()='Conditioning']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@coolantTempInlet" order="ascending"/>
				<xsl:sort data-type="number" select="@coolingPower" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template name="PowerMapTemplate" match="*[local-name()='PowerMap']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@outShaftSpeed" order="ascending"/>
				<xsl:sort data-type="number" select="@torque" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='OCV']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@SoC" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='CurrentLimits']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@SoC" order="ascending"/>
				<xsl:sort data-type="number" select="@maxChargingCurrent" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	<xsl:template match="*[local-name()='ApplicableVehicleGroups']">
		<xsl:element name="{local-name()}">
			<xsl:for-each select="*[local-name()='ApplicableVehicleGroup']">
				<xsl:sort data-type="text" select="text()" order="ascending"/>
					<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>	
	</xsl:template>	
		
	<!-- Sorts InternalResistance entries at BattterySystem component, -->
	<!-- or create the InternalResistance element at CapacitorSystem component -->
	<xsl:template match="*[local-name()='InternalResistance']">
		<xsl:choose>
			<xsl:when test="*">	
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="@*"/>
					<xsl:for-each select="*">
						<xsl:sort data-type="number" select="@SoC" order="ascending"/>
						<xsl:sort data-type="number" select="@R_2" order="ascending"/>
						<xsl:sort data-type="number" select="@R_10" order="ascending"/>
						<xsl:sort data-type="number" select="@R_20" order="ascending"/>
						<xsl:apply-templates select="."/>
					</xsl:for-each>
				</xsl:element>
			</xsl:when>
			<xsl:otherwise>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="@*|node()"/> 
				</xsl:element>
			</xsl:otherwise>		
		</xsl:choose>
	</xsl:template>		
	
	<!-- Sorts PowerMap, which has a attribute gear and sorts them by the gear number in ascending order -->
	<!-- For the components EM-IHPC and IEPC -->
	<xsl:template match="*[local-name()='PowerMap' and @gear]">
		<xsl:if test="count(preceding-sibling::*[local-name()='PowerMap']) = 0">
			<xsl:for-each select="../*[local-name()='PowerMap']">
				<xsl:sort data-type="number" select="@gear" order="ascending"/>
				<xsl:call-template name="PowerMapTemplate"/>
			</xsl:for-each>
		</xsl:if>	
	</xsl:template>	
	
	<!-- Sorts VoltageLevel, which has a child element Voltage and sorts them by this value in ascending order -->
	<!-- For the components EM, EM-IHPC and IEPC-->
	<xsl:template match="*[local-name()='VoltageLevel' and ./*[local-name()='Voltage']]">		
		<xsl:if test="count(preceding-sibling::*[local-name()='VoltageLevel']) = 0">
			<xsl:for-each select="../*[local-name()='VoltageLevel']">
				<xsl:sort data-type="number" select="./*[local-name() = 'Voltage']/text()" order="ascending"/>			
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="*"/> 	
				</xsl:element>
			</xsl:for-each>			
		</xsl:if>				
	</xsl:template>

	<!-- Sorts ElectricMachine, which has a child element Position and sorts them by this value in ascending order -->
	<!-- For SHEV_Sx vehicles with ElectricMotorTorqueLimits -->
	<xsl:template match="*[local-name()='ElectricMachine' and ./*[local-name()='Position']]">
		<xsl:if test="count(preceding-sibling::*[local-name()='ElectricMachine']) = 0">
			<xsl:for-each select="../*[local-name()='ElectricMachine']">
				<xsl:sort data-type="text" select="./*[local-name() = 'Position']/text()" order="ascending"/>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="*"/>
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>

	<!-- Sorts AngledriveType, which has an attribute axleNumber and sorts them by this value in ascending order -->
	<!-- For Multiple powertrain vehicles -->
	<xsl:template match="*[local-name()='AngledriveType' and @axleNumber]">
		<xsl:if test="count(preceding-sibling::*[local-name()='AngledriveType']) = 0">
			<xsl:for-each select="../*[local-name()='AngledriveType']">
				<xsl:sort data-type="number" select="@axleNumber" order="ascending"/>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="*"/>
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>

	<!-- Sorts RetarderType, which has an attribute axleNumber and sorts them by this value in ascending order -->
	<!-- For Multiple powertrain vehicles -->
	<xsl:template match="*[local-name()='RetarderType' and @axleNumber]">
		<xsl:if test="count(preceding-sibling::*[local-name()='RetarderType']) = 0">
			<xsl:for-each select="../*[local-name()='RetarderType']">
				<xsl:sort data-type="number" select="@axleNumber" order="ascending"/>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="*"/>
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>

	<!-- Sorts RetarderRatio, which has an attribute axleNumber and sorts them by this value in ascending order -->
	<!-- For Multiple powertrain vehicles -->
	<xsl:template match="*[local-name()='RetarderRatio' and @axleNumber]">
		<xsl:if test="count(preceding-sibling::*[local-name()='RetarderRatio']) = 0">
			<xsl:for-each select="../*[local-name()='RetarderRatio']">
				<xsl:sort data-type="number" select="@axleNumber" order="ascending"/>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="*"/>
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>

	<!-- Sorts PTO, which has an attribute axleNumber and sorts them by this value in ascending order -->
	<!-- For Multiple powertrain vehicles -->
	<xsl:template match="*[local-name()='PTO' and @axleNumber]">
		<xsl:if test="count(preceding-sibling::*[local-name()='PTO']) = 0">
			<xsl:for-each select="../*[local-name()='PTO']">
				<xsl:sort data-type="number" select="@axleNumber" order="ascending"/>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="*"/>
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>

	<!-- Sorts Powertrain, which has an attribute axleNumber and sorts them by this value in ascending order -->
	<!-- For Multiple powertrain vehicles -->
	<xsl:template match="*[local-name()='Powertrain' and @axleNumber]">
		<xsl:if test="count(preceding-sibling::*[local-name()='Powertrain']) = 0">
			<xsl:for-each select="../*[local-name()='Powertrain']">
				<xsl:sort data-type="number" select="@axleNumber" order="ascending"/>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="*"/>
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>

	<!-- Sorts FuelCellModule -->
	<!-- For FCHV vehicles -->
	<xsl:template match="*[local-name()='FuelCellModule' and ./*[local-name()='Count']]">
		<xsl:if test="count(preceding-sibling::*[local-name()='FuelCellModule']) = 0">
			<xsl:for-each select="../*[local-name()='FuelCellModule']">
				<xsl:sort data-type="number" select="./*[local-name() = 'Count']/text()" order="ascending"/>
				<xsl:sort data-type="text" select="./*[local-name() = 'FuelCell']/*[local-name() = 'Data']/@id" order="ascending"/>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="*"/>
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>

	<!-- Sorts DragCurve, which has a attribute gear and sorts them by the gear number in ascending order -->
	<!-- For the components EM IHPC and IEPC -->
	<xsl:template match="*[local-name()='DragCurve' and @gear]">
		<xsl:if test="count(preceding-sibling::*[local-name()='DragCurve']) = 0">	
			<xsl:for-each select="../*[local-name()='DragCurve']">
				<xsl:sort data-type="number" select="@gear" order="ascending"/>
				<xsl:call-template name="DragCurveTemplate"/>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>
	
	<!-- Sorts Fuel, which has a attribute type and sorts them by type string in acending order-->
	<!-- For the Engine component -->
	<xsl:template match="*[local-name()='Fuel' and @type]">
		<xsl:if test="count(preceding-sibling::*[local-name()='Fuel']) = 0">	
			<xsl:for-each select="../*[local-name()='Fuel']">
				<xsl:sort data-type="text" select="@type" order="ascending"/>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="@*|node()"/> 
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>
	
	<!-- Sorts Mode, which has a child element Fuel and sorts by the number of Fuel child elements in acending order-->
	<!-- For the Engine component-->
	<xsl:template match="*[local-name()='Mode' and ./*[local-name()='Fuel']]">
		<xsl:if test="count(preceding-sibling::*[local-name()='Mode']) = 0">
			<xsl:for-each select="../*[local-name()='Mode']">
				<xsl:sort data-type="number" select="count(./*[local-name() = 'Fuel'])" order="ascending"/>
					<xsl:element name="{local-name()}">
						<xsl:apply-templates select="@*|node()"/>
					</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template> 
	
	<!-- Sorting for Job files tags -->
		
	<!-- ElectricEnergyStorage -->
	<xsl:template match="*[local-name()='Battery' and ./*[local-name()='StringID']]">
		<xsl:if test="count(preceding-sibling::*[local-name()='Battery']) = 0">
			<xsl:for-each select="../*[local-name()='Battery']">
				<xsl:sort data-type="number" select="./*[local-name() = 'StringID']/text()" order="ascending"/>				
				<xsl:sort data-type="text" select="./*[local-name() = 'REESS']/*[local-name() = 'Data']/@id" order="ascending"/>				
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="*"/> 	
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>
			
	<!-- Sort SteeringPump Technology entries by axleNumber attribute -->
	<!-- For AUX Component SteeringPump -->
	<xsl:template match="*[local-name()='SteeringPump']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@axleNumber" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>	
	
	<!-- Sort P2.5GearRatios by gear attribute number-->
	<!-- For ElectricMachine component at HEV-Px, HEV-S2, HEV-S3, HEV-S4, PEV-E2, PEV-E3 and PEV-E4 -->
	<xsl:template match="*[local-name()='P2.5GearRatios']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@gear" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>	
	</xsl:template>
	
	<!-- Sort BoostingLimitation entries by rotationalSpeed attribute number -->
	<!-- For Vehicle HEV IEPC-S and Vehicle HEV-Px -->
	<xsl:template match="*[local-name()='BoostingLimitations']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@rotationalSpeed" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>	
	</xsl:template>
	
	<!-- Sort SmartAlternator elements by the value of the child element RatedCurrent -->
	<!-- For AUX_Conventional_PrimaryBusType ElectricSystem --> 
	<xsl:template match="*[local-name()='SmartAlternator' and ./*[local-name()='RatedCurrent']]">
		<xsl:if test="count(preceding-sibling::*[local-name()='SmartAlternator']) = 0">
			<xsl:for-each select="../*[local-name()='SmartAlternator']">
				<xsl:sort data-type="number" select="./*[local-name() = 'RatedCurrent']/text()" order="ascending"/>				
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="*"/> 	
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>
	
	<!-- Sort Battery elements by the value of the child elements BatteryTechnology and RatedCapacity-->
	<!-- For AUX_Conventional_PrimaryBusType ElectricSystem --> 
	<xsl:template match="*[local-name()='Battery' and ./*[local-name()='BatteryTechnology']]">
		<xsl:if test="count(preceding-sibling::*[local-name()='Battery']) = 0">
			<xsl:for-each select="../*[local-name()='Battery']">
				<xsl:sort data-type="text" select="./*[local-name() = 'BatteryTechnology']/text()" order="ascending"/>				
				<xsl:sort data-type="number" select="./*[local-name() = 'RatedCapacity']/text()" order="ascending"/>				
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="*"/> 	
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>
	
	<!-- Sort Capacitor elements by the value of the child elements CapacitorTechnology and RatedCapacitance -->
	<!-- For AUX_Conventional_PrimaryBusType ElectricSystem --> 
	<xsl:template match="*[local-name()='Capacitor' and ./*[local-name()='CapacitorTechnology']]">
		<xsl:if test="count(preceding-sibling::*[local-name()='Capacitor']) = 0">
			<xsl:for-each select="../*[local-name()='Capacitor']">
				<xsl:sort data-type="text" select="./*[local-name() = 'CapacitorTechnology']/text()" order="ascending"/>				
				<xsl:sort data-type="number" select="./*[local-name() = 'RatedCapacitance']/text()" order="ascending"/>				
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="*"/> 	
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>

	<xsl:template match="*[local-name()='ElectricEnergy']">
		<xsl:element name="{local-name()}">
			<xsl:for-each select="*[local-name()='EnergyConsumption']">
				<xsl:sort data-type="text" select="@unit" order="ascending"/>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="@*|node()"/>
				</xsl:element>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>


	<xsl:template match="*[local-name()='Result']//*[local-name()='Fuel' and @type]">
		<xsl:if test="count(preceding-sibling::*[local-name()='Fuel']) = 0">
			<xsl:for-each select="../*[local-name()='Fuel']">
				<xsl:sort data-type="text" select="@type" order="ascending"/>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="@*|node()">
						<xsl:sort data-type="text" select="@unit" order="ascending"/>
					</xsl:apply-templates>
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>

	<xsl:template match="*[local-name()='Summary']//*[local-name()='Fuel' and @type]">
		<xsl:if test="count(preceding-sibling::*[local-name()='Fuel']) = 0">
			<xsl:for-each select="../*[local-name()='Fuel']">
				<xsl:sort data-type="text" select="@type" order="ascending"/>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="@*|node()">
						<xsl:sort data-type="text" select="@unit" order="ascending"/>
					</xsl:apply-templates>
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>

	<xsl:template match="*[local-name()='CO2']">
		<xsl:if test="count(preceding-sibling::*[local-name()='CO2']) = 0">
			<xsl:for-each select="../*[local-name()='CO2']">
				<xsl:sort data-type="text" select="@unit" order="ascending"/>

				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="@*|node()"/>
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>

	<xsl:template match="*[local-name()='Result']//*[local-name()='OVCMode' and @type]">
		<xsl:if test="count(preceding-sibling::*[local-name()='OVCMode']) = 0">
			<xsl:for-each select="../*[local-name()='OVCMode']">
				<xsl:sort data-type="text" select="@type" order="ascending"/>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="@*|node()">
						<xsl:sort data-type="text" select="@unit" order="ascending"/>
					</xsl:apply-templates>
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>

	<xsl:template match="*[local-name()='Results']//*[local-name()='Result']">
		<xsl:if test="count(preceding-sibling::*[local-name()='Result']) = 0">
			<xsl:for-each select="../*[local-name()='Result']">
				<xsl:sort data-type="text" select="./*[local-name()='Mission']" order="ascending"/>
				<xsl:sort data-type="number" select="*[local-name()='SimulationParameters']//*[local-name()='TotalVehicleMass']" order="ascending"/>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="@*|node()"/>
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>

	<xsl:template match="*[local-name()='FC_ZEV_AuxHeater' and @type]">
		<xsl:if test="count(preceding-sibling::*[local-name()='FC_ZEV_AuxHeater']) = 0">
			<xsl:for-each select="../*[local-name()='FC_ZEV_AuxHeater']">
				<xsl:sort data-type="text" select="@type" order="ascending"/>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="@*|node()">
						<xsl:sort data-type="text" select="@unit" order="ascending"/>
					</xsl:apply-templates>
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>

	<xsl:template match="*[local-name()='CO2_ZEV_AuxHeater' and @type]">
		<xsl:if test="count(preceding-sibling::*[local-name()='CO2_ZEV_AuxHeater']) = 0">
			<xsl:for-each select="../*[local-name()='CO2_ZEV_AuxHeater']">
				<xsl:sort data-type="text" select="@type" order="ascending"/>
				<xsl:element name="{local-name()}">
					<xsl:apply-templates select="@*|node()">
						<xsl:sort data-type="text" select="@unit" order="ascending"/>
					</xsl:apply-templates>
				</xsl:element>
			</xsl:for-each>
		</xsl:if>
	</xsl:template>

</xsl:transform>
