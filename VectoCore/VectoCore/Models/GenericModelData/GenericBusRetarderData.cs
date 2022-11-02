using System;
using System.Data;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.Declaration
{
	public class GenericBusRetarderData
	{

		public RetarderData CreateGenericBusRetarderData(IRetarderInputData retarderInput) =>
			new RetarderData {
				Type = retarderInput?.Type ?? RetarderType.None,
				Ratio = retarderInput?.Ratio ?? 1.0,
				LossMap = retarderInput?.Type.IsDedicatedComponent() ?? false 
					? GenerateGenericLossMap(retarderInput.Ratio) : null
			};

		private RetarderLossMap GenerateGenericLossMap(double stepUpRatio)
		{
			var retarderSpeeds = new double[] {
					0, 200 , 400, 600, 900, 1200,
					1600, 2000, 2500, 3000, 3500, 4000,
					4500, 5000
				};

			var genericRetarderLosses = GetHydrodynamicRetardersLoss(retarderSpeeds, stepUpRatio);
			//var genericRetarderLosses = GetMagneticRetarderLoss(retarderSpeeds, stepUpRatio);

			var torqueLoss = new DataTable();
			torqueLoss.Columns.Add(RetarderLossMapReader.Fields.RetarderSpeed);
			torqueLoss.Columns.Add(RetarderLossMapReader.Fields.TorqueLoss);

			for (int i = 0; i < genericRetarderLosses.Length; i++)
			{
				var newRow = torqueLoss.NewRow();
				newRow[RetarderLossMapReader.Fields.RetarderSpeed] = Math.Round(retarderSpeeds[i], 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow[RetarderLossMapReader.Fields.TorqueLoss] = Math.Round(genericRetarderLosses[i] *
					Constants.GenericLossMapSettings.RetarderGenericFactor, 2, MidpointRounding.AwayFromZero).ToXMLFormat();
				torqueLoss.Rows.Add(newRow);
			}

			return RetarderLossMapReader.Create(torqueLoss);
		}

		private double[] GetHydrodynamicRetardersLoss(double[] retarderSpeeds, double stepUpRatio)
		{
			var losses = new double[retarderSpeeds.Length];

			for (int i = 0; i < retarderSpeeds.Length; i++)
			{
				losses[i] = 10.0 / stepUpRatio + 2.0 / Math.Pow(stepUpRatio, 3) * Math.Pow(retarderSpeeds[i] / 1000.0, 2);
			}

			return losses;
		}

		private double[] GetMagneticRetarderLoss(double[] retarderSpeeds, double stepUpRatio)
		{
			var losses = new double[retarderSpeeds.Length];

			for (int i = 0; i < retarderSpeeds.Length; i++)
			{
				losses[i] = 15.0 / stepUpRatio + 2.0 / Math.Pow(stepUpRatio, 4) * Math.Pow(retarderSpeeds[i] / 1000.0, 3);
			}

			return losses;
		}
	}
}
