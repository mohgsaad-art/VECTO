using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Folding;

namespace VECTO3GUI.Helper
{
	public sealed class AvalonEditBehaviour : Behavior<TextEditor>
	{
		public static readonly DependencyProperty CurrentTextProperty =
			DependencyProperty.Register("CurrentText", typeof(string), typeof(AvalonEditBehaviour),
				new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

		public string CurrentText
		{
			get { return (string)GetValue(CurrentTextProperty); }
			set { SetValue(CurrentTextProperty, value); }
		}

		private void AssociatedObjectOnTextChanged(object sender, EventArgs eventArgs)
		{
			var textEditor = sender as TextEditor;
			if (textEditor != null)
			{
				if (textEditor.Document != null)
					CurrentText = textEditor.Document.Text;
			}
		}

		private static void PropertyChangedCallback(DependencyObject dependencyObject,
			DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
		{
			var behavior = dependencyObject as AvalonEditBehaviour;
			if (behavior.AssociatedObject != null)
			{
				var editor = behavior.AssociatedObject as TextEditor;



				if (editor.Document != null)
				{
					var caretOffset = editor.CaretOffset;
					editor.Document.Text = dependencyPropertyChangedEventArgs.NewValue.ToString();
					editor.CaretOffset = caretOffset;
				}


				var foldingManager = FoldingManager.Install(editor.TextArea);
				var foldingStrategy = new XmlFoldingStrategy();
				foldingStrategy.UpdateFoldings(foldingManager, editor.Document);
			}
		}
	}
}
