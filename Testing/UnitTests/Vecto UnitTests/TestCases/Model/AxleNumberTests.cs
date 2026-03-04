using System.Reflection.Metadata;
using Microsoft.VisualBasic;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using Constants = TUGraz.VectoCore.Configuration.Constants;

namespace TUGraz.Vecto.UnitTests.TestCases.Model;

public class AxleNumberTests
{
    [Test]
    public void CreateAxleNumber()
    {
        var axleNumber = new AxleNumber(9);
        Assert.That(axleNumber.Value, Is.EqualTo(9));
    }

    [Test]
    public void CreateNotInPowertrain()
    {
        var axleNumber = AxleNumber.NotInAxlePowertrain();
        Assert.That(axleNumber.Value, Is.EqualTo(-1));
    }

    [Test]
    public void EqualOperator()
    {
        var axleNumber = AxleNumber.NotInAxlePowertrain();
        var axleNumber2 = AxleNumber.NotInAxlePowertrain();

        Assert.That(axleNumber == axleNumber2);
    }

    [TestCase(1, 4)]
    [TestCase(2, 2)]
    [TestCase(-1, 3)]
    [TestCase(-1, -1)]
    [TestCase(-1, -2)]
    public void EqualOperator2(int a, int b)
    {
        var axleNumber = new AxleNumber(a);
        var axleNumber2 = new AxleNumber(b);

        if (a == b)
        {
            Assert.That(axleNumber == axleNumber2);
        }
        else
        {
            Assert.That(axleNumber != axleNumber2);
        }
    }

    [Test]
    public void EqualOperator3()
    {
        AxleNumber a = null;
        AxleNumber b = new AxleNumber(4);
        Assert.Multiple(() =>
        {
            Assert.That(!(a == b));
            Assert.That(!(b == a));
            Assert.That(a != b);
            Assert.That(b != a);


            int? iA = null;
            int? iB = null;

            b = null;
            Assert.That((a == b) == (iA == iB));
            Assert.That((a != b) == (iA != iB));

            Assert.That((b == a) == (iB == iA));
            Assert.That((b != a) == (iB != iA));
        });
    }



    [TestCase(3, 4)]
    [TestCase(4, 4)]
    [TestCase(-1, 4)]
    [TestCase(0, 0)]
    public void IntegerEqualOperator(int a, int b)
    {
        var axleB = new AxleNumber(b);

        Assert.That((a == axleB) == (a == b));
        Assert.That((axleB == a) == (b == a));

        Assert.That(!(axleB == null));
    }

    // [TestCase(-1)]
    // [TestCase(4)]
    // [TestCase(0)]
    // public void AssignmentOperatorTest(int a)
    // {
    //     AxleNumber axleNumber = a;
    //
    //     Assert.That(axleNumber.Value, Is.EqualTo(a));
    //
    //
    //     int aAsInteger = axleNumber;
    //     Assert.That(aAsInteger == a);
    // }


    [Test]
    public void OrderTest()
    {
        var intList = new List<int>() { 9, 872, -1, 4873245, 1273012, 234935802, 7187234, 8, 7 };
        var orderedAxleList = intList.Select(i => new AxleNumber(i)).OrderBy(ax => ax).ToList();
        Assert.That(intList.OrderBy(i => i).SequenceEqual(orderedAxleList.Select(ax => ax.Value)));
    }



}
    
    