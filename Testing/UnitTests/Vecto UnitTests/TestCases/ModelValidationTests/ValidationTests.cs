using System.ComponentModel.DataAnnotations;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.ModelValidationTests;

public class ValidationTests
{
	/// <summary>
	/// VECTO-107 Check valid range of input parameters
	/// </summary>
	[TestCase]
	public void Validation_Test()
	{
		var results = new DataObject().Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, null, false);

		// every field and property should be tested except private parent fields and properties and 
		// (4*4+1) * 2 = 17*2= 34 - 4 private parent fields (+2 public field and property which are tested twice) = 32
		Assert.AreEqual(32, results.Count, "Validation Error: " + results.Select(r => r.ErrorMessage).Join("\n_eng_avg"));
	}

	[TestCase]
	public void ValidateDictionaryTest()
	{
		var container = new ContainerObject() {
			Elements = new Dictionary<int, WrapperObject>() {
				{ 2, new WrapperObject() { Value = 41 } },
				{ 4, new WrapperObject() { Value = -30 } }
			}
		};

		var results = container.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, null, false);
		Assert.AreEqual(1, results.Count);
	}

	[TestCase]
	public void ValidateDoubleErrorTest()
	{
		var wrap = new WrapperObject() { Value = 101 };
		var container = new ContainerObject() {
			Elements = new Dictionary<int, WrapperObject>() {
				{ 1, wrap },
				{ 2, wrap }
			}
		};

		var results = container.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, null, false);
		Assert.AreEqual(1, results.Count);

	}

    public class ContainerObject
    {
        [Required, ValidateObject] public Dictionary<int, WrapperObject> Elements;
    }

    public class WrapperObject
    {
        [Required, System.ComponentModel.DataAnnotations.Range(0, 100)] public int Value = 0;
    }

    public class DeepDataObject
    {
        [Required, System.ComponentModel.DataAnnotations.Range(41, 42)] protected int public_field = 5;
    }

    public abstract class ParentDataObject
    {
        #region 4 parent instance fields

        // ReSharper disable once NotAccessedField.Local
        [Required, System.ComponentModel.DataAnnotations.Range(1, 2)] private int private_parent_field = 7;
        [Required, System.ComponentModel.DataAnnotations.Range(3, 4)] protected int protected_parent_field = 7;
        [Required, System.ComponentModel.DataAnnotations.Range(5, 6)] internal int internal_parent_field = 7;
        [Required, System.ComponentModel.DataAnnotations.Range(7, 8)] public int public_parent_field = 5;

        #endregion

        #region 4 parent static field

        [Required, System.ComponentModel.DataAnnotations.Range(43, 44)] private static int private_static_parent_field = 7;
        [Required, System.ComponentModel.DataAnnotations.Range(43, 44)] protected static int protected_static_parent_field = 7;
        [Required, System.ComponentModel.DataAnnotations.Range(50, 51)] internal static int internal_static_parent_field = 7;
        [Required, System.ComponentModel.DataAnnotations.Range(45, 46)] public static int public_static_parent_field = 7;

        #endregion

        #region 4 parent instance properties

        [Required, System.ComponentModel.DataAnnotations.Range(11, 12)]
        private int private_parent_property => 7;

        [Required, System.ComponentModel.DataAnnotations.Range(13, 14)]
        protected int protected_parent_property => 7;

        [Required, System.ComponentModel.DataAnnotations.Range(15, 16)]
        internal int internal_parent_property => 7;

        [Required, System.ComponentModel.DataAnnotations.Range(17, 18)]
        public int public_parent_property => 7;

        #endregion

        #region 4 parent static properties

        [Required, System.ComponentModel.DataAnnotations.Range(19, 20)]
        private static int private_static_parent_property => 7;

        [Required, System.ComponentModel.DataAnnotations.Range(19, 20)]
        protected static int protected_static_parent_property => 7;

        [Required, System.ComponentModel.DataAnnotations.Range(19, 20)]
        internal static int internal_static_parent_property => 7;

        [Required, System.ComponentModel.DataAnnotations.Range(19, 20)]
        public static int public_static_parent_property => 7;

        #endregion

        #region 1 parent sub objects

        [Required, ValidateObject] public DeepDataObject parent_sub_object = new DeepDataObject();

        #endregion

        private void just_to_remove_compiler_warnings()
        {
            private_parent_field = private_static_parent_field;
        }
    }

    public class DataObject : ParentDataObject
    {
        #region 4 instance fields

        // ReSharper disable once NotAccessedField.Local
        [Required, System.ComponentModel.DataAnnotations.Range(1, 2)] private int private_field = 7;
        [Required, System.ComponentModel.DataAnnotations.Range(3, 4)] protected int protected_field = 7;
        [Required, System.ComponentModel.DataAnnotations.Range(5, 6)] internal int internal_field = 7;
        [Required, System.ComponentModel.DataAnnotations.Range(7, 8)] public int public_field = 5;

        #endregion

        #region 4 static field

        [Required, System.ComponentModel.DataAnnotations.Range(43, 44)] private static int private_static_field = 7;
        [Required, System.ComponentModel.DataAnnotations.Range(43, 44)] protected static int protected_static_field = 7;
        [Required, System.ComponentModel.DataAnnotations.Range(50, 51)] internal static int internal_static_field = 7;
        [Required, System.ComponentModel.DataAnnotations.Range(45, 46)] public static int public_static_field = 7;

        #endregion

        #region 4 instance properties

        [Required, System.ComponentModel.DataAnnotations.Range(11, 12)]
        private int private_property => 7;

        [Required, System.ComponentModel.DataAnnotations.Range(13, 14)]
        protected int protected_property => 7;

        [Required, System.ComponentModel.DataAnnotations.Range(15, 16)]
        internal int internal_property => 7;

        [Required, System.ComponentModel.DataAnnotations.Range(17, 18)]
        public int public_property => 7;

        #endregion

        #region 4 static properties

        [Required, System.ComponentModel.DataAnnotations.Range(19, 20)]
        private static int private_static_property => 7;

        [Required, System.ComponentModel.DataAnnotations.Range(19, 20)]
        protected static int protected_static_property => 7;

        [Required, System.ComponentModel.DataAnnotations.Range(19, 20)]
        internal static int internal_static_property => 7;

        [Required, System.ComponentModel.DataAnnotations.Range(19, 20)]
        public static int public_static_property => 7;

        #endregion

        #region 1 sub objects

        [Required, ValidateObject] public DeepDataObject sub_object = new DeepDataObject();

        #endregion

        private void just_to_remove_compiler_warnings()
        {
            private_field = private_static_field;
        }
    }
}