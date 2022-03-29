
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
	
	<!-- Sorts Fuel, which has a attribute type and sorts them by type string in ascending order-->
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
	
	<!-- Sorts Mode, which has a child element Fuel and sorts by the number of Fuel child elements in ascending order-->
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
	
	<!-- Sorting for Job files -->
		
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
			
	<!-- ElectricMotorTorqueLimits -->	
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
	
	<!-- AUX SteeringPump -->
	<xsl:template match="*[local-name()='SteeringPump']">
		<xsl:element name="{local-name()}">
			<xsl:apply-templates select="@*"/>
			<xsl:for-each select="*">
				<xsl:sort data-type="number" select="@axleNumber" order="ascending"/>
				<xsl:apply-templates select="."/>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>	
	

	

</xsl:transform>
