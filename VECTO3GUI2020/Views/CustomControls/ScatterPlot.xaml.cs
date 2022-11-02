using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Security.RightsManagement;
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
using InteractiveDataDisplay.WPF;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI2020.Properties;
using Color = System.Drawing.Color;

namespace VECTO3GUI2020.Views.CustomControls
{
    /// <summary>
    /// Interaction logic for ScatterPlot.xaml
    /// </summary>
    public partial class ScatterPlot : UserControl
    {


        public bool SwitchAxis
        {
            get { return (bool)GetValue(SwitchAxisProperty); }
            set { SetValue(SwitchAxisProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SwitchAxis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SwitchAxisProperty =
            DependencyProperty.Register("SwitchAxis", typeof(bool), typeof(ScatterPlot), new PropertyMetadata(false));



        public DataTable MyDataTable
        {
            get { return (DataTable)GetValue(MyDataTableProperty); }
            set { SetValue(MyDataTableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyDataTableProperty =
            DependencyProperty.Register("MyDataTable", 
				typeof(DataTable), 
				typeof(ScatterPlot), new FrameworkPropertyMetadata(
					new DataTable("DefaultDataTable"), 
					FrameworkPropertyMetadataOptions.None,
					ContentChanged));

		private ResourceManager _resourceManager;

		private string _yLabel;
		private string _xLabel;

		private DataColumn _xColumn;
		private DataColumn _yColumn;
		private DataColumn _cColumn;
		private DataColumn _sColumn;

        private bool _hasThirdCol = false;




		public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ScatterPlot), new PropertyMetadata("Title"));




        public string LeftTitle
        {
            get { return (string)GetValue(LeftTitleProperty); }
            set { SetValue(LeftTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftTitleProperty =
            DependencyProperty.Register("LeftTitle", typeof(string), typeof(ScatterPlot), new PropertyMetadata("y_axis_label"));





        public string BottomTitle
        {
            get { return (string)GetValue(BottomTitleProperty); }
            set { SetValue(BottomTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BottomTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomTitleProperty =
            DependencyProperty.Register("BottomTitle", typeof(string), typeof(ScatterPlot), new PropertyMetadata("x_axis_label"));




        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Description.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(ScatterPlot), new PropertyMetadata(null));




        public string ColorDescription
        {
            get { return (string)GetValue(ColorDescriptionProperty); }
            set { SetValue(ColorDescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorDescription.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorDescriptionProperty =
            DependencyProperty.Register("ColorDescription", typeof(string), typeof(ScatterPlot), new PropertyMetadata(""));



        public ScatterPlot()
        {
			InitializeComponent();
			_resourceManager = Strings.ResourceManager;
		}




		private static void ContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
            ScatterPlot plot = (ScatterPlot)d;
			if (plot.MyDataTable == null || plot.MyDataTable.TableName == "DefaultDataTable")
			{
				return;
			}
            
            plot.SetColumns();
			plot.SetLabels();
		}

		private void SetColumns()
		{

			Debug.Assert(MyDataTable.Columns.Count > 2);
			if (SwitchAxis) {
				_xColumn = MyDataTable.Columns[1];
				_yColumn = MyDataTable.Columns[0];
			} else {
				_xColumn = MyDataTable.Columns[0];
				_yColumn = MyDataTable.Columns[1];
			}

			_hasThirdCol = MyDataTable.Columns.Count >= 3;


			uint N = (uint)MyDataTable.Rows.Count;
			object[] xData = new object[N];
			object[] yData = new object[N];

			if (_hasThirdCol) {
				uint i = 0;
				object[] cData = new object[N];
				_cColumn = MyDataTable.Columns[2];
				foreach (DataRow row in MyDataTable.Rows)
				{
					xData[i] = row[_xColumn].ToString().ToDouble();
					yData[i] = row[_yColumn].ToString().ToDouble();
					cData[i] = row[_cColumn].ToString().ToDouble();
					i++;
				}
				circles.PlotColor(xData, yData, cData);
			} else {
				uint i = 0;
				foreach (DataRow row in MyDataTable.Rows)
				{
					xData[i] = row[_xColumn].ToString().ToDouble();
					yData[i] = row[_yColumn].ToString().ToDouble();
					i++;
				}
				circles.PlotXY(xData, yData);
			}
		}

		private void SetLabels()
		{
			Title = MyDataTable.TableName;
			LeftTitle = _yColumn.ColumnName;
			BottomTitle = _xColumn.ColumnName;
			Description = _yColumn.ColumnName;
			if (_hasThirdCol) {
				ColorDescription = _cColumn.ColumnName;
			}



		}



        /*
		 * double[] x = new double[N];
            double[] y = new double[N];
            double[] c = new double[N];
            double[] d = new double[N];

            Random rand = new Random();
            //circles
            for (int i = 0; i < N; i++)
            {
                x[i] = rand.Next(M);
                y[i] = rand.Next(M);
                c[i] = rand.NextDouble();
                d[i] = 20 * rand.NextDouble();
            }

            circles.PlotColorSize(x, y, c, d);
            //diamonds
            x = new double[N];
            y = new double[N];
            c = new double[N];
            d = new double[N];

            for (int i = 0; i < N; i++)
            {
                x[i] = rand.Next(M);
                y[i] = rand.Next(M);
                c[i] = rand.NextDouble();
                d[i] = 20 * rand.NextDouble();
            }
            diamonds.PlotColorSize(x, y, c, d);
		 *
		 *
		 *
		 *
		 *
		 */

    }
}
