﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Vts;
using Vts.SiteVisit.Input;
using Vts.SiteVisit.ViewModel;
using Vts.Extensions;
//using SLExtensions.Input;

namespace Vts.SiteVisit.View
{
    public partial class SolverDemoView : UserControl
    {
        public static SolverDemoView Current = null;

        private FloatableWindow _floatableWindow; 
        public SolverDemoView()
        {
            InitializeComponent();

            Current = this;

            _floatableWindow = new FloatableWindow()
            {
                Name = "wndIsolatedStorageView",
                Content = new IsolatedStorageView(),
                ParentLayoutRoot = this.layoutRoot,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            Commands.IsoStorage_IncreaseSpaceQuery.Executed += IsoStorage_IncreaseSpaceQuery_Executed;
        }

        void IsoStorage_IncreaseSpaceQuery_Executed(object sender, SLExtensions.Input.ExecutedEventArgs e)
        {
            _floatableWindow.ShowDialog();
        }

        private void inputTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var inputTab = sender as TabControl;
            if (inputTab != null && inputTab.Items != null &&
                outputTabControl !=null && outputTabControl.Items != null && outputTabControl.Items.Count > 1)
            {
                var tabItem = inputTab.SelectedItem as TabItem;
                if (tabItem != null)
                {
                    switch (tabItem.Name)
                    {
                        case "tabForward":
                        case "tabInverse":
                        case "tabSpectral":
                        default:
                            outputTabControl.SelectedItem = outputTabControl.Items[0];
                            ((TabItem)outputTabControl.Items[0]).Visibility = Visibility.Visible;
                            ((TabItem)outputTabControl.Items[1]).Visibility = Visibility.Collapsed;
                            break;
                        case "tabFluence":
                            outputTabControl.SelectedItem = outputTabControl.Items[1];
                            ((TabItem)outputTabControl.Items[1]).Visibility = Visibility.Visible;
                            ((TabItem)outputTabControl.Items[0]).Visibility = Visibility.Collapsed;
                            break;
                        case "tabMonteCarlo":
                            ((TabItem)outputTabControl.Items[1]).Visibility = Visibility.Visible;
                            ((TabItem)outputTabControl.Items[0]).Visibility = Visibility.Visible;
                            break;
                    }
                }
            }
        }
    }
}
