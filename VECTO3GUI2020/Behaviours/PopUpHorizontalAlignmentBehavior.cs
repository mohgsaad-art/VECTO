using System.Windows;
using System.Windows.Controls.Primitives;
using Microsoft.Xaml.Behaviors;

namespace VECTO3GUI2020.Behaviours
{
	public class PopUpHorizontalAlignmentBehavior : Behavior<Popup>
	{

		private FrameworkElement _placementTarget;
		private double _initialHorizontalOffset;
		private double _popUpWidth;



		private void SetHorizontalAlignment(double placeMentTargetActualWidth, double popUpActualWidth)
		{
			if (popUpActualWidth > placeMentTargetActualWidth) {
				this.AssociatedObject.HorizontalOffset = placeMentTargetActualWidth - popUpActualWidth;
			}
		}
		private void AssociatedObject_Opened(object sender, System.EventArgs e)
		{

			var popUpWidth = this.AssociatedObject.ActualWidth;
			if (this.AssociatedObject.ActualWidth == 0 && this.AssociatedObject.IsMeasureValid) {
				popUpWidth = this.AssociatedObject.DesiredSize.Width;
			}

			//SetHorizontalAlignment(_placementTarget.ActualWidth, this.AssociatedObject.ActualWidth != 0 ? : this.AssociatedObject.ActualWidth);
		}


		#region Overrides of Behavior


		protected override void OnAttached()
		{
			_placementTarget = this.AssociatedObject.PlacementTarget as FrameworkElement;
			_popUpWidth = this.AssociatedObject.MinWidth;
            this.AssociatedObject.Opened += AssociatedObject_Opened;
			base.OnAttached();
		}



		protected override void OnDetaching()
		{
			this.AssociatedObject.HorizontalOffset = _initialHorizontalOffset;
			base.OnDetaching();
		}



		#endregion
	}
}