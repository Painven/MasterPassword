using MasterPasswordDesktop.Commands;
using MasterPasswordDesktop.DataAccess;
using MasterPasswordDesktop.Infrastructure;
using MasterPasswordDesktop.Model;
using MasterPasswordDesktop.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace MasterPasswordDesktop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        const string SEARCH_PLACEHOLDER_TEXT = "Поиск";
        public EditDataLineWindowViewModel EditWindowViewModel { get; } = new EditDataLineWindowViewModel();

        ObservableCollection<DataLineViewModel> _items;
        public ObservableCollection<DataLineViewModel> Items 
        { 
            get => _items; 
            set => Set(ref _items, value); 
        }

        string _title;
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        string _searchText = SEARCH_PLACEHOLDER_TEXT;
        public string SearchText { get => _searchText; set => Set(ref _searchText, value); }

        bool _isLoading = true;
        public bool IsLoading
        {
            get => _isLoading;
            set => Set(ref _isLoading, value);
        }

        bool _useRegex = false;
        public bool UseRegex
        {
            get => _useRegex;
            set
            {
                Set(ref _useRegex, value);
                collectionView.Refresh();
            }
        }

        int _selectedIndex = 1;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => Set(ref _selectedIndex, value);
        }

        DataLineViewModel _selectedItem;
        public DataLineViewModel SelectedItem { get => _selectedItem; set => Set(ref _selectedItem, value); }

        ICollectionView collectionView;

        public ICommand CloseApplicationCommand { get; }
        public ICommand DeleteSelectedDataLineCommand { get; }
        public ICommand AddNewDataLineCommand { get; }
        public ICommand EditDataLineCommand { get; }
        public ICommand ShowGeneratePasswordWindowCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearSearchTextPlaceholderCommand { get; }
        public ICommand RecoverSearchTextPlaceholderCommand { get; }
        public ICommand LoadedCommand { get; }

        public MainWindowViewModel()
        {
            SearchCommand = new LambdaCommand((p) => collectionView.Refresh());
            CloseApplicationCommand = new CloseApplicationCommand();
            AddNewDataLineCommand = new LambdaCommand((o) => ShowDataLineWindow(new DataLine())); 
            EditDataLineCommand = new LambdaCommand((o) => ShowDataLineWindow((DataLine)o));
            DeleteSelectedDataLineCommand = new LambdaCommand(DeleteSelectedDataLine, (o) => (o != null));
            ShowGeneratePasswordWindowCommand = new LambdaCommand(ShowGeneratePasswordWindow);
            ClearSearchTextPlaceholderCommand = new LambdaCommand(ClearSearchPlaceholder);
            RecoverSearchTextPlaceholderCommand = new LambdaCommand(RecoverySearchPlaceholder);
            LoadedCommand = new LambdaCommand(async e => await Loaded());

            EditWindowViewModel.OnSave += (e) =>
            {
                e.EditDate = DateTime.Now;
                e.LastViewDate = DateTime.Now;
                if (e.Id == Guid.Empty)
                {
                    e.Id = Guid.NewGuid();
                }
                App.Database.AddOrUpdateDataLine(e);
            };
            EditWindowViewModel.OnCancel += (e) =>
            {
                if (e.Id != Guid.Empty)
                {
                    e.LastViewDate = DateTime.Now;
                    App.Database.AddOrUpdateDataLine(e);
                }
                Items.Where(i => i.DataLine.Id == Guid.Empty).ToList().ForEach(item => Items.Remove(item));
            };
        }

        private async Task Loaded()
        {
            //await Task.Delay(TimeSpan.FromSeconds(0.25));
            IEnumerable<DataLineViewModel> items = null;

            await Task.Run(() =>
            {
                items = App.Database
                    .ReadDataLines()
                    .Where(dl => dl.Id != Guid.Empty)
                    .OrderByDescending(dl => dl.LastViewDate)
                    .Select(dl => new DataLineViewModel(dl))
                    .ToList();
                items.ToList().ForEach(item =>
                {
                    item.OnEditButtonPressed += ShowDataLineWindow;
                    item.OnDataCopy += Item_OnDataCopy;
                });
            });


            Items = new ObservableCollection<DataLineViewModel>(items);



            collectionView = CollectionViewSource.GetDefaultView(Items);
            collectionView.Filter = SearchPredicate;
            IsLoading = false;
        }

        private void Item_OnDataCopy(DataLine o)
        {
            o.LastViewDate = DateTime.Now;
            App.Database.AddOrUpdateDataLine(o);
        }

        private bool SearchPredicate(object o)
        {
            if (string.IsNullOrWhiteSpace(SearchText) || SearchText.Equals(SEARCH_PLACEHOLDER_TEXT)) return true;

            var item = (o as DataLineViewModel).DataLine;
            var properties = typeof(DataLine).GetProperties().Where(pi => pi.PropertyType == typeof(string)).ToList();

            if (UseRegex)
            {
                if (properties.Any(pi => pi.GetValue(item) != null && Regex.IsMatch(pi.GetValue(item).ToString(), SearchText, RegexOptions.IgnoreCase)))
                {
                    return true;
                }
            }
            else
            {
                if (properties.Any(pi => pi.GetValue(item) != null && pi.GetValue(item).ToString().IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    return true;
                }
            }


            return false;
        }

        private void AddItem(DataLine dataLine)
        {
            var item = new DataLineViewModel(dataLine);
            if (!Items.Contains(item))
            {
                item.OnEditButtonPressed += ShowDataLineWindow;
                Items.Add(item);
            }
        }

        private void ShowDataLineWindow(DataLine item)
        {
            bool appendNew = string.IsNullOrWhiteSpace(item.Title);
            if (appendNew)
            {
                AddItem(item);
            }           

            var window = new Views.EditDataLineWindow();
            EditWindowViewModel.Item = item;
            window.DataContext = EditWindowViewModel;
            window.Show();

        }

        private void ShowGeneratePasswordWindow(object obj)
        {
            var window = new Views.GeneratePasswordWindow();
            window.ShowDialog();
        }

        private void DeleteSelectedDataLine(object dataLine)
        {
            SelectedItem.OnEditButtonPressed -= ShowDataLineWindow;
            App.Database.DeleteDataLine((dataLine as DataLineViewModel).DataLine);
            Items.Remove(SelectedItem);                     
        }

        private void RecoverySearchPlaceholder(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchText)) { SearchText = SEARCH_PLACEHOLDER_TEXT; }
        }

        private void ClearSearchPlaceholder(object obj)
        {
            if (SearchText == SEARCH_PLACEHOLDER_TEXT) { SearchText = string.Empty; }
        }
    }
}
