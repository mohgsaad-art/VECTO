Imports NUnit.Framework


Namespace UnitTests

    <TestFixture()>
    Public Class PneumaticConsumerTests

        <Test()>
        Public Sub CreateNewTest()
            Assert.Fail()
        End Sub

        <Test(), ExpectedException("System.ArgumentException")>
        Public Sub CreateNewInvalidNameTest()
            'Names cannot be zero length
            Assert.Fail()
        End Sub

        <Test(), ExpectedException("System.ArgumentOutOfRangeException")>
        Public Sub CreateNewInvalidVolumeTest()
            'Initially, zero volume is invalid TODO: Need to check with Pascal etc for valid bounds that make sense
            Assert.Fail()
        End Sub


        <Test()>
        Public Sub GetVolumePerCycleTest()
            Assert.Fail()
        End Sub

        <Test()>
        Public Sub GetNameTest()
            Assert.Fail()
        End Sub

        <Test()>
        Public Sub GetTotalVolumeTest()
            Assert.Fail()
        End Sub

    End Class
End Namespace


