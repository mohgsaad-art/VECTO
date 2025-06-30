/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;

namespace TUGraz.VectoCommon.Utils
{
	/// <summary>
	/// Helper Class for doing the Validation
	/// </summary>
	public static class ValidationHelper
	{

		//private static List<KeyValuePair<Object, List<ValidationResult>>> _validationHistory = new List<KeyValuePair<Object, List<ValidationResult>>>();
		private static ConcurrentDictionary<Object, List<ValidationResult>> _validationHistory = new ConcurrentDictionary<Object, List<ValidationResult>>();

		private static ConcurrentDictionary<Object, ValidationAttribute[]> propDictionary = new ConcurrentDictionary<object, ValidationAttribute[]>();

		private static ConcurrentDictionary<Object, ValidationAttribute[]> fieldDictionary = new ConcurrentDictionary<object, ValidationAttribute[]>();



		public static void ClearValHistory()
		{
			_validationHistory.Clear();
		}
		/// <summary>
		/// Validates the specified entity and all its properties recursively. (Extension Method)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity">The entity.</param>
		/// <param name="mode">validate the entity for the given execution mode</param>
		/// <param name="jobType"></param>
		/// <param name="emPosition"></param>
		/// <param name="gbxType"></param>
		/// <param name="emsCycle"></param>
		/// <returns>Null, if the validation was successfull. Otherwise a list of ValidationResults with the ErrorMessages.</returns>
		public static IList<ValidationResult> Validate<T>(this T entity, ExecutionMode mode,
			VectoSimulationJobType jobType, PowertrainPosition? emPosition, GearboxType? gbxType,
			bool emsCycle)
		{
			if (entity == null) {
				return new[] { new ValidationResult($"null value given for {typeof(T)}") };
			}
			
			var results = new List<ValidationResult>();
			var context = new ValidationContext(entity);
			context.InitializeServiceProvider(type => new VectoValidationModeServiceContainer(mode, jobType, emPosition, gbxType, emsCycle));

			// Check if Obj is from TUGraz Namespace
			if (!entity.GetType().ToString().Contains(("TUGraz")))
				return results;

			if (_validationHistory.ContainsKey(entity)) 
				return _validationHistory[entity];

			


			Validator.TryValidateObject(entity, context, results, true);

			const BindingFlags flags =
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public |
				BindingFlags.FlattenHierarchy;

			var properties = entity.GetType().GetProperties(flags);
			foreach (var p in properties) {

				
				ValidationAttribute[] attributes = propDictionary.ContainsKey(p) ? propDictionary[p] : p.GetAttributes<ValidationAttribute>(entity.GetType()).ToArray();

				//ValidationAttribute[] attributes = p.GetAttributes<ValidationAttribute>(entity.GetType()).ToArray();
				
				if (!propDictionary.ContainsKey(p))
					propDictionary.TryAdd(p, attributes);

				if (attributes.Any()) {
					var val = p.GetValue(entity);

					context.DisplayName = p.Name;
					context.MemberName = p.Name;

					Validator.TryValidateValue(val, context, results, attributes);
				}
			}

			var fields = entity.GetType().GetFields(flags);
			foreach (var f in fields) {
				
				ValidationAttribute[] attributes = fieldDictionary.ContainsKey(f) ? fieldDictionary[f] : f.GetAttributes<ValidationAttribute>(entity.GetType()).ToArray();
				
				//ValidationAttribute[] attributes = f.GetAttributes<ValidationAttribute>(entity.GetType()).ToArray();

				if (!fieldDictionary.ContainsKey(f))
					fieldDictionary.TryAdd(f, attributes);

				if (attributes.Any()) {
					var val = f.GetValue(entity);
					context.DisplayName = f.Name;
					context.MemberName = f.Name;
					
					Validator.TryValidateValue(val, context, results, attributes);
				}
			}
			
			_validationHistory.TryAdd(entity, results);
			return results;
		}

		private static bool CheckTUGrazNamespace(Object obj)
		{
			if(obj.GetType().ToString().Contains("TUGraz"))
				return true;
			return false;
		}

		/// <summary>
		/// Gets the attributes of a member for the current class, parent classes and all interfaces.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="m"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		private static IEnumerable<T> GetAttributes<T>(this MemberInfo m, Type obj) where T : Attribute
		{
			var attributes = Enumerable.Empty<T>();

			const BindingFlags flags =
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public |
				BindingFlags.FlattenHierarchy;

			if (m.MemberType == MemberTypes.Property) {
				PropertyInfo prop = m as PropertyInfo ?? obj.GetProperty(m.Name, flags);
				//var prop = obj.GetProperty(m.Name, flags);
				if (prop != null)
				{
					//if (m is PropertyInfo prop) {
					attributes = prop.GetCustomAttributes(typeof(T))
						.Cast<T>()
						.Concat(obj.GetInterfaces().SelectMany(m.GetAttributes<T>));
				}
			}

			if (m.MemberType == MemberTypes.Field) {
				FieldInfo field = m as FieldInfo ?? obj.GetField(m.Name, flags);
				//var field = obj.GetField(m.Name, flags);
				if (field != null)
				{
					//if (m is FieldInfo field) {
					attributes =
						attributes.Concat(
							field.GetCustomAttributes(typeof(T)).Cast<T>().Concat(obj.GetInterfaces().SelectMany(m.GetAttributes<T>))).Distinct();
				}
			}
            

			return attributes;
		}

		/// <summary>
		/// Determines whether this instance is valid. (Extension Method)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="entity">The entity.</param>
		/// <returns></returns>
		public static bool IsValid<T>(this T entity)
		{
			return Validator.TryValidateObject(entity, new ValidationContext(entity), null, true);
		}
	}

	public class VectoValidationModeServiceContainer
	{
		public ExecutionMode Mode { get; protected set; }

		public VectoSimulationJobType JobType { get; protected set; }

		public PowertrainPosition EMPowertrainPosition { get; protected set; }
		public GearboxType? GearboxType { get; protected set; }
		public bool IsEMSCycle { get; protected set; }
		public List<ValidationHistoryItem> History { get; protected set; }


		public VectoValidationModeServiceContainer(ExecutionMode mode, VectoSimulationJobType jobType, PowertrainPosition? emPosition, GearboxType? gbxType, bool isEMSCycle = false)
		{
			Mode = mode;
			GearboxType = gbxType;
			IsEMSCycle = isEMSCycle;
			JobType = jobType;
			EMPowertrainPosition = emPosition ?? PowertrainPosition.HybridPositionNotSet;
		}
	}

	public class ValidationHistoryItem
	{
		private (Object, List<ValidationResult>) historyItem;

		public ValidationHistoryItem(Object entity, List<ValidationResult> result)
		{
			historyItem = (entity, result);
		}
	}


	/// <summary>
	/// Determines that the attributed object should be validated recursively.
	/// </summary>
	public class ValidateObjectAttribute : ValidationAttribute
	{
		/// <summary>
		/// Validates an object recursively (all elements if its a list, or all fields and properties if its an object).
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <param name="validationContext">The context information about the validation operation.</param>
		/// <returns>
		/// ValidationResult.Success if the validation was successfull. Otherwise the joined ErrorMessages are returned.
		/// </returns>
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (value == null) {
				return ValidationResult.Success;
			}

			var validationService =
				validationContext.GetService(typeof(VectoValidationModeServiceContainer)) as VectoValidationModeServiceContainer;
			var mode = validationService?.Mode ?? ExecutionMode.Declaration;
			var gbxType = validationService != null ? validationService.GearboxType : GearboxType.MT;
			var isEmsCycle = validationService != null && validationService.IsEMSCycle;
			var jobType = validationService?.JobType ?? VectoSimulationJobType.ConventionalVehicle;
			var emPos = validationService?.EMPowertrainPosition;

			var enumerable = value as IEnumerable;
			if (enumerable != null) {
				var i = 0;
				foreach (var element in enumerable) {
					if (element != null) {
						var valueType = element.GetType();
						if (valueType.IsGenericType) {
							var baseType = valueType.GetGenericTypeDefinition();
							if (baseType == typeof(KeyValuePair<,>)) {
								var kvResults = new List<ValidationResult>();
								kvResults.AddRange(valueType.GetProperty("Key").GetValue(element).Validate(mode, jobType, emPos, gbxType, isEmsCycle));
								kvResults.AddRange(valueType.GetProperty("Value").GetValue(element).Validate(mode, jobType, emPos, gbxType, isEmsCycle));
								if (kvResults.Any()) {
									return new ValidationResult(
										string.Format("{1}[{0}] in {1} invalid: {2}", valueType.GetProperty("Key").GetValue(element),
											validationContext.DisplayName, kvResults.Join("\n")));
								}
							}
						}

						var results = element.Validate(mode, jobType, emPos, gbxType, isEmsCycle);
						if (results.Any()) {
							return new ValidationResult(
								$"{validationContext.DisplayName}[{i}] in {validationContext.DisplayName} invalid: {results.Join("\n")}");
						}
					}
					i++;
				}
			} else {
				var results = value.Validate(mode, jobType, emPos, gbxType, isEmsCycle);
				if (!results.Any()) {
					return ValidationResult.Success;
				}
				var messages = results.Select(r => r.MemberNames.Distinct().Join());
				if (validationContext.MemberName == "Container" || validationContext.MemberName == "RunData") {
					return new ValidationResult(results.Join("\n"), messages);
				}
				return new ValidationResult(
					$"{{{validationContext.DisplayName}}} invalid: {results.Join("\n")}", messages);
			}

			return ValidationResult.Success;
		}
	}

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
	public class RangeOrNaN : RangeAttribute
	{
		public override bool IsValid(object value)
		{
			return double.IsNaN((double)value) || base.IsValid(value);
		}

		public RangeOrNaN(double minimum, double maximum) : base(minimum, maximum) {}
	}

	/// <summary>
	/// Attribute which validates the Min-Max Range of an SI Object.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
	public class SIRangeAttribute : RangeAttribute
	{
		private ExecutionMode? _mode;
		private bool? _emsMission;
		private string _unit = "-";

		/// <summary>
		/// Checks the Min-Max Range of SI Objects.
		/// </summary>
		/// <param name="minimum">The minimum.</param>
		/// <param name="maximum">The maximum.</param>
		/// <param name="mode">if specified the validation is only performed in the corresponding mode</param>
		public SIRangeAttribute(int minimum, int maximum, ExecutionMode mode)
			: base(minimum, maximum)
		{
			_mode = mode;
		}

		/// <summary>
		/// Checks the Min-Max Range of SI Objects.
		/// </summary>
		/// <param name="minimum">The minimum.</param>
		/// <param name="maximum">The maximum.</param>
		public SIRangeAttribute(int minimum, int maximum)
			: base(minimum, maximum) {}

		/// <summary>
		/// Checks the Min-Max Range of SI Objects.
		/// </summary>
		/// <param name="minimum">The minimum.</param>
		/// <param name="maximum">The maximum.</param>
		/// <param name="mode">if specified the validation is only performed in the corresponding mode</param>
		public SIRangeAttribute(double minimum, double maximum, ExecutionMode mode)
			: base(minimum, maximum)
		{
			_mode = mode;
		}

		/// <summary>
		/// Checks the Min-Max Range of SI Objects.
		/// </summary>
		/// <param name="minimum">The minimum.</param>
		/// <param name="maximum">The maximum.</param>
		public SIRangeAttribute(double minimum, double maximum)
			: base(minimum, maximum) {}

		/// <summary>
		/// Checks the Min-Max Range of SI Objects.
		/// </summary>
		/// <param name="minimum">The minimum.</param>
		/// <param name="maximum">The maximum.</param>
		/// <param name="mode">if specified the validation is only performed in the corresponding mode</param>
		public SIRangeAttribute(SI minimum, SI maximum, ExecutionMode mode)
			: base(minimum.Value(), maximum.Value())
		{
			_mode = mode;
		}

		/// <summary>
		/// Checks the Min-Max Range of SI Objects.
		/// </summary>
		/// <param name="minimum">The minimum.</param>
		/// <param name="maximum">The maximum.</param>
		public SIRangeAttribute(SI minimum, SI maximum)
			: base(minimum.Value(), maximum.Value()) {}

		/// <summary>
		/// Checks the Min-Max Range of SI Objects.
		/// </summary>
		/// <param name="minimum">The minimum.</param>
		/// <param name="maximum">The maximum.</param>
		/// <param name="mode">if specified the validation is only performed in the corresponding mode</param>
		/// <param name="emsMission">Validation only applies if the mission is an EMS mission</param>
		public SIRangeAttribute(int minimum, int maximum, ExecutionMode mode, bool emsMission)
			: base(minimum, maximum)
		{
			_mode = mode;
			_emsMission = emsMission;
		}

		/// <summary>
		/// Checks the Min-Max Range of SI Objects.
		/// </summary>
		/// <param name="minimum">The minimum.</param>
		/// <param name="maximum">The maximum.</param>
		/// <param name="emsMission">Validation only applies if the mission is an EMS mission</param>
		public SIRangeAttribute(int minimum, int maximum, bool emsMission) : base(minimum, maximum)
		{
			_emsMission = emsMission;
		}

		/// <summary>
		/// Checks the Min-Max Range of SI Objects.
		/// </summary>
		/// <param name="minimum">The minimum.</param>
		/// <param name="maximum">The maximum.</param>
		/// <param name="mode">if specified the validation is only performed in the corresponding mode</param>
		/// <param name="emsMission">Validation only applies if the mission is an EMS mission</param>
		public SIRangeAttribute(double minimum, double maximum, ExecutionMode mode, bool emsMission)
			: base(minimum, maximum)
		{
			_mode = mode;
			_emsMission = emsMission;
		}

		/// <summary>
		/// Checks the Min-Max Range of SI Objects.
		/// </summary>
		/// <param name="minimum">The minimum.</param>
		/// <param name="maximum">The maximum.</param>
		/// <param name="emsMission">Validation only applies if the mission is an EMS mission</param>
		public SIRangeAttribute(double minimum, double maximum, bool emsMission) : base(minimum, maximum)
		{
			_emsMission = emsMission;
		}

		/// <summary>
		/// Checks the Min-Max Range of SI Objects.
		/// </summary>
		/// <param name="minimum">The minimum.</param>
		/// <param name="maximum">The maximum.</param>
		/// <param name="mode">if specified the validation is only performed in the corresponding mode</param>
		/// <param name="emsMission">Validation only applies if the mission is an EMS mission</param>
		public SIRangeAttribute(SI minimum, SI maximum, ExecutionMode mode, bool emsMission)
			: base(minimum.Value(), maximum.Value())
		{
			_mode = mode;
			_emsMission = emsMission;
		}

		/// <summary>
		/// Checks the Min-Max Range of SI Objects.
		/// </summary>
		/// <param name="minimum">The minimum.</param>
		/// <param name="maximum">The maximum.</param>
		/// <param name="emsMission">Validation only applies if the mission is an EMS mission</param>
		public SIRangeAttribute(SI minimum, SI maximum, bool emsMission) : base(minimum.Value(), maximum.Value())
		{
			_emsMission = emsMission;
		}

		/// <summary>
		/// Validates that an SI Object is inside the min-max range.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <param name="validationContext">The context information about the validation operation.</param>
		/// <returns>
		/// ValidationResult.Success if the validation was successfull, otherwise an Instance of ValidationResult with the ErrorMessage.
		/// </returns>
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var si = value as SI;

			if (si != null) {
				_unit = si.UnitString;
			}
			var validationService =
				validationContext.GetService(typeof(VectoValidationModeServiceContainer)) as VectoValidationModeServiceContainer;
			var mode = validationService?.Mode;
			var emsMode = validationService != null && validationService.IsEMSCycle;

			if (!_emsMission.HasValue || _emsMission.Value == emsMode) {
				if (mode == null) {
					return base.IsValid(si != null ? si.Value() : value, validationContext);
				}
				if (_mode == null || (_mode != null && (_mode.Value == mode))) {
					return base.IsValid(si != null ? si.Value() : value, validationContext);
				}
				return ValidationResult.Success;
			}
			return ValidationResult.Success;
		}

		public override string FormatErrorMessage(string name)
		{
			const string unitString = "{0} [{1}]";

			return string.Format(ErrorMessageString, name, string.Format(unitString, Minimum, _unit),
				string.Format(unitString, Maximum, _unit));
		}
	}

	/// <summary>
	/// Attribute which validates a Path.
	/// </summary>
	public class PathAttribute : ValidationAttribute
	{
		/// <summary>
		/// Validates that a path actually exists.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <param name="validationContext">The context information about the validation operation.</param>
		/// <returns>
		/// ValidationResult.Success if the validation was successfull, otherwise an Instance of ValidationResult with the ErrorMessage.
		/// </returns>
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (!File.Exists((string)value)) {
				return new ValidationResult("File not found: " + (string)value);
			}
			return ValidationResult.Success;
		}
	}
}