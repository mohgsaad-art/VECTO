using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl.FuelCell
{
	//public class Window<TEntry>
	//{
	//	public IEnumerable<TEntry> Entries
	//	{
	//		get
	//		{

	//		}
	//	}
	//}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TEntry"></typeparam>
	/// <typeparam name="TIterable">Type of variable to iterate over</typeparam>
	public class GeneralizedModDataWindowIterator<TEntry, TIterable> // : IEnumerator<Window<TEntry>>
		where TIterable : SIBase<TIterable>
	{



		/// <summary>
		/// TODO: update this class with generic math (.net 6)
		/// </summary>
		private readonly Func<TEntry, TIterable> getX;
		private readonly Func<TEntry, TIterable> getDeltaX;
        private readonly List<TEntry> _entries;
		private readonly int _numEntries;
		private readonly TIterable _windowSize;


		private readonly TIterable _startDistance;
        private readonly TIterable _distance;
		private readonly TIterable _endDistance;

		public int Position { get; protected set; }
		protected internal int Start { get; protected set; }
		protected internal int End { get; protected set; }
		public bool WindowEndReached { get; protected set; }


		#region Implementation of IEnumerator
		//bool IEnumerator.MoveNext()
		//{
		//	if (!EndReached) {
		//		MoveNext();
		//		return true;
		//	}

		//	return false;
		//}

		//public void Reset()
		//{
		//	throw new NotImplementedException();
		//}

		//Window<TEntry> IEnumerator<Window<TEntry>>.Current {
		//	get
		//	{

		//	}
		//}


		#endregion


		public int CurrentIndex { get; protected set; }
		protected bool IsOnStart = true;

		public bool EndReached { get; protected set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="windowSize"></param>
        /// <param name="getX"></param>
        /// <exception cref="VectoException"></exception>
        public GeneralizedModDataWindowIterator(IEnumerable<TEntry> entries, 
			TIterable windowSize,
			Expression<Func<TEntry, TIterable>> getX, Expression<Func<TEntry, TIterable>> getDeltaX)
		{
			//ModData = results;
			_entries = entries.ToList();

			//NumRows = ModData.Data.Rows.Count;
			_numEntries = _entries.Count();

			//WindowDistance = windowDistance;
			_windowSize = windowSize;

			

            //DistanceColumn = results.Data.Columns[ModalResultField.dist.GetShortCaption()];
            this.getX = getX.Compile();
			this.getDeltaX = getDeltaX.Compile();


	

            //CycleStartDistance = (Meter)results.Data.Rows[0][DistanceColumn] - (Meter)results.Data.Rows[0][ModalResultField.simulationDistance.GetName()];
            _startDistance = this.getX(_entries.First()) - this.getDeltaX(_entries.First());
			_endDistance = this.getX(_entries.Last());

			_distance = (TIterable) (_endDistance - _startDistance);


            if (windowSize.IsGreaterOrEqual(_distance)) 
			{
				throw new VectoException("Window size must be smaller than total cycle distance", ErrorCodes.FUELCELL_WINDOW_SIZE_TOO_BIG);
			}

			SetIteratorPositions(0);
		}


		public virtual void MoveNext()
		{
			if (EndReached)
			{
				//return;
				throw new VectoException("Failed to move iterator forward - iterated all elements", ErrorCodes.FUELCELL_MODALDATA_OUT_OF_BOUNDS);

			}

			IsOnStart = false;

			var newStart = GetNextRow(Position);
			SetIteratorPositions(newStart);
			//var nextWindow = GetNextRow(Position);
			if (Position == 0)
			{
				EndReached = true;
			}
		}




        protected void SetIteratorPositions(int position)
		{
			//Position = position;
			Position = position;

			//var currentDistance = (Meter)ModData.Data.Rows[Position][DistanceColumn];
			var currentDistance = getX(_entries[position]);

			var startDistance = GetDistanceInCycle(currentDistance - _windowSize / 2.0);
			var endDistance = GetDistanceInCycle(currentDistance + _windowSize / 2.0);

			if (IsOnStart)
			{
				End = MoveIteratorForward(Position, i => getX(_entries[i]).IsSmallerOrEqual(endDistance));
				Start = MoveIteratorBackward(GetPreviousRow(Position), i => getX(_entries[i]).IsSmallerOrEqual(startDistance));

			}
            else
			{
				if (endDistance.IsSmaller(getX(_entries[End])))
				{
					End = MoveIteratorForward(0, i => getX(_entries[i]).IsSmallerOrEqual(endDistance));
				}
                else
				{
					End = MoveIteratorForward(End, r =>
					{
						if (endDistance.IsEqual(_distance) && r == 0)
						{
							return false;
						}
						return getX(_entries[r]).IsSmallerOrEqual(endDistance);
					});
				}

				if (startDistance.IsSmaller(getX(_entries[Start])))
				{
					Start = MoveIteratorForward(0, r => getX(_entries[r]).IsSmallerOrEqual(startDistance));
				}
				else {
					Start = GetNextRow(MoveIteratorForward(Start, r => getX(_entries[r]).IsSmallerOrEqual(startDistance)));
				}
			}

			CurrentIndex = Start;
			WindowEndReached = false;
		}



		protected virtual int MoveIteratorForward(int startIdx, Func<int, bool> action)
		{
			var retVal = startIdx;
			var initialValue = action(startIdx);
			do
			{
				retVal = GetNextRow(retVal);
				if (action(retVal) != initialValue)
				{
					return GetPreviousRow(retVal);
				}
			} while (retVal != startIdx);

			return retVal;
		}

		protected virtual int MoveIteratorBackward(int startIdx, Func<int, bool> action)
		{
			var retVal = startIdx;
			var initialValue = action(startIdx);
			do
			{
				retVal = GetPreviousRow(retVal);
				if (action(retVal) != initialValue)
				{
					return GetNextRow(retVal);
				}

				if (retVal == startIdx)
				{
					throw new VectoException("Failed to move iterator forward - iterated all elements", ErrorCodes.FUELCELL_MODALDATA_OUT_OF_BOUNDS);
				}
			} while (true);
		}




        protected virtual SIBase<TIterable> GetDistanceInCycle(TIterable relativeDistance)
		{
			//var cycleDistance = ModData.Distance;

			var totalDistance = _distance;

			while (relativeDistance.IsGreater(_startDistance + totalDistance))
			{
				relativeDistance -= totalDistance;
			}

			while (relativeDistance.IsSmaller(_startDistance))
			{
				relativeDistance += totalDistance;
			}
			return relativeDistance;
		}


		protected virtual int GetNextRow(int rowIdx)
		{
			return (rowIdx + 1) % _numEntries;

		}

		protected virtual int GetPreviousRow(int rowIdx)
		{
			var retVal = (rowIdx - 1) % _numEntries;
			if (retVal < 0)
			{
				retVal += _numEntries;
			}
			return retVal;
		}

		public virtual void NextEntry()
		{
			if (WindowEndReached)
			{
				return;
			}
			CurrentIndex = GetNextRow(CurrentIndex);
			if (CurrentIndex == GetNextRow(End))
			{
				WindowEndReached = true;
			}
		}

		#region Implementation of IDisposable

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		#endregion
	}







    public class ModDataWindowIterator //: IEnumerator<DataRow>
	{
		public ModalDataContainer ModData { get; set; }

		protected DataColumn DistanceColumn { get; }

		//protected ModDataAsRingBuffer RingBuffer { get; }

		protected Meter WindowDistance { get; }

		protected int NumRows;
		public int Position { get; protected set; }
		protected internal int Start { get; protected set; }
		protected internal int End { get; protected set; }
		//bool IEnumerator.MoveNext()
		//{

		//	if (CycleEndReached) {
		//		return false;
		//	}
		//	MoveNext();
		//	return true;


		//}

		//public void Reset()
		//{
		//	SetIteratorPositions(0);
		//}

		//DataRow IEnumerator<DataRow>.Current => throw new NotImplementedException();

		//object IEnumerator.Current { get; }


		public int Current { get; protected set; }

		protected bool CycleStart = true;

		public bool CycleEndReached { get; protected set; } = false;

		public bool WindowEndReached { get; protected set; } = false;

		public Meter CycleStartDistance { get; set; }

		public ModDataWindowIterator(ModalDataContainer results, Meter windowDistance)
		{
			ModData = results;
			NumRows = ModData.Data.Rows.Count;

			WindowDistance = windowDistance;

			DistanceColumn = results.Data.Columns[ModalResultField.dist.GetShortCaption()];
			CycleStartDistance = (Meter)results.Data.Rows[0][DistanceColumn] - (Meter)results.Data.Rows[0][ModalResultField.simulationDistance.GetName()];


			if (windowDistance.IsGreaterOrEqual(ModData.Distance))
			{
				throw new VectoException("Window size must be smaller than total cycle distance", ErrorCodes.FUELCELL_WINDOW_SIZE_TOO_BIG);
			}

			SetIteratorPositions(0);
		}


		public virtual void MoveNext()
		{
			if (CycleEndReached)
			{
				//return;
				throw new VectoException("Failed to move iterator forward - iterated all elements", ErrorCodes.FUELCELL_MODALDATA_OUT_OF_BOUNDS);

			}

			CycleStart = false;

			var newStart = GetNextRow(Position);
			SetIteratorPositions(newStart);
			//var nextWindow = GetNextRow(Position);
			if (Position == 0)
			{
				CycleEndReached = true;
			}
		}

		public virtual void NextEntry()
		{
			if (WindowEndReached)
			{
				return;
			}
			Current = GetNextRow(Current);
			if (Current == GetNextRow(End))
			{
				WindowEndReached = true;
			}
		}

		public override string ToString()
		{

			return
				$"ModDataWindow: wndSize: {WindowDistance}, Position: {Position} ({ModData.Data.Rows[Position][DistanceColumn]} " +
				$"Start: {Start} ({ModData.Data.Rows[Start][DistanceColumn]}) " +
				$"End: {End} ({ModData.Data.Rows[End][DistanceColumn]}) " +
				$"ActualWindowSize: {ActualWindowSize}";
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public virtual Meter ActualWindowSize
		{
			get
			{
				var startDistance = (Meter)ModData.Data.Rows[Start][DistanceColumn];
				var endDistance = (Meter)ModData.Data.Rows[End][DistanceColumn];
				var diff = endDistance - startDistance;

				return diff < 0 ? diff + ModData.Distance : diff;
			}
		}

		protected void SetIteratorPositions(int position)
		{
			Position = position;

			var currentDistance = (Meter)ModData.Data.Rows[Position][DistanceColumn];
			var startDistance = GetDistanceInCycle(currentDistance - WindowDistance / 2.0);
			var endDistance = GetDistanceInCycle(currentDistance + WindowDistance / 2.0);

			if (CycleStart)
			{
				End = MoveIteratorForward(Position, r => (Meter)ModData.Data.Rows[r][DistanceColumn] <= endDistance);
				Start = MoveIteratorBackward(GetPreviousRow(Position), r => (Meter)ModData.Data.Rows[r][DistanceColumn] <= startDistance);
			}
			else
			{
				if (endDistance < (Meter)ModData.Data.Rows[End][DistanceColumn])
				{
					End = MoveIteratorForward(0, r => (Meter)ModData.Data.Rows[r][DistanceColumn] <= endDistance);
				}
				else
				{
					End = MoveIteratorForward(End, r =>
					{
						if (endDistance.IsEqual(ModData.Distance) && r == 0)
						{
							return false;
						}
						return (Meter)ModData.Data.Rows[r][DistanceColumn] <= endDistance;
					});
				}

				if (startDistance < (Meter)ModData.Data.Rows[Start][DistanceColumn])
				{
					Start = MoveIteratorForward(0, r => (Meter)ModData.Data.Rows[r][DistanceColumn] <= startDistance);
				}
				else
				{
					Start = GetNextRow(MoveIteratorForward(Start,
						r => (Meter)ModData.Data.Rows[r][DistanceColumn] <= startDistance));
				}
			}

			Current = Start;
			WindowEndReached = false;
		}

		protected virtual SIBase<Meter> GetDistanceInCycle(Meter relativeDistance)
		{
			var cycleDistance = ModData.Distance;

			while (relativeDistance.IsGreater(CycleStartDistance + cycleDistance))
			{
				relativeDistance -= cycleDistance;
			}

			while (relativeDistance.IsSmaller(CycleStartDistance))
			{
				relativeDistance += cycleDistance;
			}
			return relativeDistance;
		}

		protected virtual int MoveIteratorForward(int startIdx, Func<int, bool> action)
		{
			var retVal = startIdx;
			var initialValue = action(startIdx);
			do
			{
				retVal = GetNextRow(retVal);
				if (action(retVal) != initialValue)
				{
					return GetPreviousRow(retVal);
				}
			} while (retVal != startIdx);

			return retVal;
		}


		protected virtual int MoveIteratorBackward(int startIdx, Func<int, bool> action)
		{
			var retVal = startIdx;
			var initialValue = action(startIdx);
			do
			{
				retVal = GetPreviousRow(retVal);
				if (action(retVal) != initialValue)
				{
					return GetNextRow(retVal);
				}

				if (retVal == startIdx)
				{
					throw new VectoException("Failed to move iterator forward - iterated all elements", ErrorCodes.FUELCELL_MODALDATA_OUT_OF_BOUNDS);
				}
			} while (true);
		}

		protected virtual int GetNextRow(int rowIdx)
		{
			return (rowIdx + 1) % NumRows;

		}

		protected virtual int GetPreviousRow(int rowIdx)
		{
			var retVal = (rowIdx - 1) % NumRows;
			if (retVal < 0)
			{
				retVal += NumRows;
			}
			return retVal;
		}

    }
}