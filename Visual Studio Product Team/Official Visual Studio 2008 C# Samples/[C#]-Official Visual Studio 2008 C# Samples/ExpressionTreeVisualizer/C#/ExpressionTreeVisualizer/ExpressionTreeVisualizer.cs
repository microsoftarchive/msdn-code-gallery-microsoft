using System;
using System.Text;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.CSharp;
using System.Collections;
using System.Linq.Expressions;
using Microsoft.VisualStudio.DebuggerVisualizers;
using System.Diagnostics;
using System.Runtime.Serialization;
using ExpressionVisualizer;

[assembly: DebuggerVisualizer(typeof(ExpressionTreeVisualizer), typeof(ExpressionTreeVisualizerObjectSource), Target = typeof(Expression), Description = "Expression Tree Visualizer")]

namespace ExpressionVisualizer
{

    public class ExpressionTreeVisualizer : DialogDebuggerVisualizer
    {
        private IDialogVisualizerService modalService;

        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {

            modalService = windowService;
            if (modalService == null)
                throw new NotSupportedException("This debugger does not support modal visualizers");

            ExpressionTreeContainer container = (ExpressionTreeContainer)objectProvider.GetObject();
            TreeBrowser browser = new TreeBrowser();
            browser.Add(container.Tree);
            TreeWindow treeForm = new TreeWindow(browser, container.Expression);
            modalService.ShowDialog(treeForm);
        }
    }
}