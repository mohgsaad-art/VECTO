using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components
{
    public abstract class GearViewModel : ViewModelBase, IComponentViewModel, IGearViewModel
    {
        IComponentViewModelFactory _vmFactory;
		protected IXMLGearData _inputData;



		public abstract void SetProperties();

		public double MaxTorqueLossCalculated => GetMaxTorqueLossFromTable();
		public double MinTorqueLossCalculated => GetMinTorqueLossFromTable();


		public double MaxInputSpeedCalculated => GetMaxInputSpeedFromTable();
		public double MinInputSpeedCalculated => GetMinInputSpeedFromTable();

		public double MaxInputTorqueCalculated => GetMaxInputSpeedFromTable();

		public double MinInputTorqueCalculated => GetMinInputTorqueFromTable();


		public DataView LossMapDataView => _lossMap.DefaultView;

		public GearViewModel(IXMLGearData inputData)
        {
            _inputData = inputData as IXMLGearData;
            Debug.Assert(_inputData != null);


            SetProperties();
			CalculateEfficiency();
		}

        

        private void CalculateEfficiency()
        {
            DataColumn efficiencyColumn = new DataColumn("Efficiency", typeof(double));
            LossMap.Columns.Add(efficiencyColumn);
            foreach (DataRow row in _lossMap.Rows)
            {
                row[efficiencyColumn.ColumnName] = Math.Round((row.Field<string>("Torque Loss").ToDouble() + row.Field<string>("Input Torque").ToDouble()) / row.Field<string>("Input Torque").ToDouble(),2);
            }
		}

        
        private double GetMinTorqueLossFromTable()
        {
            return _lossMap.Columns["Torque Loss"].Values<String>().ToDouble().Min();
        }
		private double GetMaxTorqueLossFromTable()
        {
            return _lossMap.Columns["Torque Loss"].Values<String>().ToDouble().Max(); ;
        }
		private double GetMinInputSpeedFromTable()
        {
            return _lossMap.Columns["Input Speed"].Values<String>().ToDouble().Min();
        }
		private double GetMaxInputSpeedFromTable()
        {
            return _lossMap.Columns["Input Speed"].Values<String>().ToDouble().Max();
        }
		private double GetMaxInputTorqueFromTable()
        {
            return _lossMap.Columns["Torque Input"].Values<String>().ToDouble().Max();
        }
		private double GetMinInputTorqueFromTable()
        {
            return _lossMap.Columns["Torque Input"].Values<String>().ToDouble().Min();
            
        }

		protected string _name = "gear";
		public string Name => _name;

		public bool IsPresent => true;


        #region Implementation of ITransmissionInputData
        protected int _gear;
		protected double _ratio;
		protected TableData _lossMap;
		protected double _efficiency;
		protected NewtonMeter _maxTorqueInput;
		protected PerSecond _maxInputSpeed;
		protected TableData _shiftPolygon;
		protected DataSource _dataSource;

		public virtual int Gear {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual double Ratio {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual TableData LossMap {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual double Efficiency {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual NewtonMeter MaxTorque {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual PerSecond MaxInputSpeed {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual TableData ShiftPolygon {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public virtual DataSource DataSource {get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        #endregion
    }



    public class GearViewModel_v1_0 : GearViewModel
    {

        public static readonly string VERSION = typeof(XMLGearDataV10).FullName;
        public GearViewModel_v1_0(IXMLGearData inputData) : base(inputData)
        {

        }

		public override void SetProperties()
		{
			throw new NotImplementedException();
		}
	}

    public class GearViewModel_v2_0 : GearViewModel_v1_0
    {
        public static readonly string VERSION = typeof(XMLGearDataV20).FullName;


		public GearViewModel_v2_0(IXMLGearData inputData) : base(inputData)
        {

        }

		public override void SetProperties()
		{
			_name += " " + _gear;
			_gear = _inputData.Gear;
			_ratio = _inputData.Ratio;
			_lossMap = _inputData.LossMap;
			_lossMap.TableName = "Loss Map";
			_efficiency = _inputData.Efficiency;
			_maxTorqueInput = _inputData.MaxTorque;
			_maxInputSpeed = _inputData.MaxInputSpeed;
			_shiftPolygon = _inputData.ShiftPolygon;
		}

        #region Override Properties
        public override int Gear
		{
			get => _gear;
			set => SetProperty(ref _gear, value);
		}

		public override double Ratio
		{
			get => _ratio;
			set => SetProperty(ref _ratio, value);
		}

		public override TableData LossMap
		{
			get => _lossMap;
			set => SetProperty(ref _lossMap, value);
		}

		public override double Efficiency
		{
			get => _efficiency;
			set => SetProperty(ref _efficiency, value);
		}

		public override NewtonMeter MaxTorque
		{
			get => _maxTorqueInput;
			set => SetProperty(ref _maxTorqueInput, value);
		}

		public override PerSecond MaxInputSpeed
		{
			get => _maxInputSpeed;
			set => SetProperty(ref _maxInputSpeed, value);
		}

		public override TableData ShiftPolygon
		{
			get => _shiftPolygon;
			set => SetProperty(ref _shiftPolygon, value);
		}

        #endregion
    }
}
