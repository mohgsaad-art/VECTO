using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VECTO3GUI.Views.CustomControls
{
	/// <summary>
	/// Interaction logic for CommonComponentData.xaml
	/// </summary>
	public partial class CommonDeclarationComponentData : UserControl, INotifyDataErrorInfo
	{
		public CommonDeclarationComponentData()
		{
			InitializeComponent();
			Validation.SetErrorTemplate(this, null);
			(Content as FrameworkElement).DataContext = this;
		}



		public string Manufacturer
		{
			get { return (string)GetValue(ManufacturerProperty); }
			set { SetValue(ManufacturerProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Manufacturer.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ManufacturerProperty =
			DependencyProperty.Register("Manufacturer", typeof(string), typeof(CommonDeclarationComponentData), new PropertyMetadata(""));



		public string Model
		{
			get { return (string)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Model.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ModelProperty =
			DependencyProperty.Register("Model", typeof(string), typeof(CommonDeclarationComponentData), new PropertyMetadata(""));



		public string CertificationNumber
		{
			get { return (string)GetValue(CertificationNumberProperty); }
			set { SetValue(CertificationNumberProperty, value); }
		}

		// Using a DependencyProperty as the backing store for CertificationNumber.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CertificationNumberProperty =
			DependencyProperty.Register("CertificationNumber", typeof(string), typeof(CommonDeclarationComponentData), new PropertyMetadata(""));



		public DateTime Date
		{
			get { return (DateTime)GetValue(DateProperty); }
			set { SetValue(DateProperty, value); }
		}

		// Using a DependencyProperty as the backing store for Date.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty DateProperty =
			DependencyProperty.Register("Date", typeof(DateTime), typeof(CommonDeclarationComponentData), new PropertyMetadata(null));





		public delegate void ErrorsChangedEventHandler(object sender, DataErrorsChangedEventArgs e);

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		public bool HasErrors
		{
			get {
				var validationErrors = Validation.GetErrors(this);
				return validationErrors.Any();
			}
		}

		public IEnumerable GetErrors(string propertyName)
		{
			var validationErrors = Validation.GetErrors(this);
			var specificValidationErrors =
				validationErrors.Where(
					error => ((BindingExpression)error.BindingInError).TargetProperty.Name == propertyName).ToList();
			var specificValidationErrorMessages = specificValidationErrors.Select(valError => valError.ErrorContent);
			return specificValidationErrorMessages;
		}

		public void NotifyErrorsChanged(string propertyName)
		{
			if (ErrorsChanged != null) {
				ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
			}
		}

		protected static void ValidatePropertyWhenChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			((VectoParameterControl)dependencyObject).NotifyErrorsChanged(dependencyPropertyChangedEventArgs.Property.Name);
		}
	}
}
