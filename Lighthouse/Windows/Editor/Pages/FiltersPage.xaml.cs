﻿using LighthouseLibrary.Models;
using LighthouseLibrary.Models.Enums;
using LighthouseLibrary.Services;
using System;
using System.Collections.Generic;
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

namespace Lighthouse.Windows.Editor.Pages
{
    /// <summary>
    /// Interaction logic for Filters.xaml
    /// </summary>
    public partial class FiltersPage : Page
    {
        public FiltersPage()
        {
            InitializeComponent();

            List<Filter> items = new List<Filter>
            {
                new Filter(UtilService.GenerateNewId(), FilterType.GaussianBlur),
                new Filter(UtilService.GenerateNewId(), FilterType.None)
            };

            icFilters.ItemsSource = items;
        }
    }
}
