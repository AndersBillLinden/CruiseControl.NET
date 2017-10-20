<?xml version="1.0"?>
<xsl:stylesheet
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

    <xsl:output method="html"/>
    
    <xsl:template match="/">
    			<xsl:variable name="pass_toplevel" select="count(/cruisecontrol/build/Tests/UnitTestResult[outcome=10])"/>
    			<xsl:variable name="pass_multiples" select="count(/cruisecontrol/build/Tests/UnitTestResult/innerResults/element[outcome=10])"/>
    			<xsl:variable name="pass_count" select="$pass_toplevel + $pass_multiples"/>
    			<xsl:variable name="inconclusive_toplevel" select="count(/cruisecontrol/build/Tests/UnitTestResult[outcome=4])"/>
    			<xsl:variable name="inconclusive_multiples" select="count(/cruisecontrol/build/Tests/UnitTestResult/innerResults/element[outcome=4])"/>
    			<xsl:variable name="inconclusive_count" select="$inconclusive_toplevel + $inconclusive_multiples"/>
    			<xsl:variable name="failed_toplevel" select="count(/cruisecontrol/build/Tests/UnitTestResult[outcome=1])"/>
    			<xsl:variable name="failed_multiples" select="count(/cruisecontrol/build/Tests/UnitTestResult/innerResults/element[outcome=1])"/>
    			<xsl:variable name="failed_count" select="$failed_toplevel + $failed_multiples"/>
			<xsl:variable name="total_count" select="$failed_count + $pass_count + $inconclusive_count"/>
    </xsl:template>
    
    <xsl:template match="UnitTestResult">
			<xsl:variable name="testId" select="/cruisecontrol/build/Tests/UnitTestResult/id/testId/id"/>
			<xsl:variable name="testDetails" select="/cruisecontrol/build/Tests/TestRun/tests/value[id=$testId]"/>
    </xsl:template>
</xsl:stylesheet>
