using Syncfusion.Maui.DataGrid;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ContextMenuSample
{
    public class OrderInfoRepository : INotifyPropertyChanged
    {
        private ObservableCollection<OrderInfo>? orderInfo;

        public ObservableCollection<OrderInfo> OrderInfoCollection
        {
            get => orderInfo!;
            set
            {
                if (orderInfo != value)
                {
                    orderInfo = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SortAscendingCommand { get; }
        public ICommand SortDescendingCommand { get; }
        public ICommand ClearSortingCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand UndoDeleteCommand { get; }
        private OrderInfo? _lastDeletedItem;
        private int _lastDeletedIndex = -1;
        public ICommand ExpandGroupCommand { get; }
        public ICommand CollapseGroupCommand { get; }
        public ICommand ShowTotalOrdersCommand { get; }
        public ICommand ShowTotalGroupsCommand { get; }

        public OrderInfoRepository()
        {
            OrderInfoCollection = new ObservableCollection<OrderInfo>();
            GenerateOrders();
            SortAscendingCommand = new Command<HeaderContextInfo>(OnSortAscending);
            SortDescendingCommand = new Command<HeaderContextInfo>(OnSortDescending);
            ClearSortingCommand = new Command<HeaderContextInfo>(OnClearSorting);
            DeleteCommand = new Command<RowContextMenuInfo>(OnDelete);
            UndoDeleteCommand = new Command<RowContextMenuInfo>(OnUndoDelete);
            ExpandGroupCommand = new Command<GroupCaptionContextInfo>(OnExpandGroup);
            CollapseGroupCommand = new Command<GroupCaptionContextInfo>(OnCollapseGroup);
            ShowTotalOrdersCommand = new Command<TableSummaryContextInfo>(OnShowTotalOrders);
            ShowTotalGroupsCommand = new Command<TableSummaryContextInfo>(OnShowTotalGroups);
        }

        private void OnShowTotalGroups(TableSummaryContextInfo info)
        {
            if (info?.DataGrid == null) return;

            var grid = info.DataGrid;
            int totalGroups = grid.View?.TopLevelGroup?.Groups?.Count ?? 0;
            var row = info.SummaryRow ?? grid.TableSummaryRows.FirstOrDefault();
            if (row == null) return;

            row.ShowSummaryInRow = true;
            row.Title = $"Total Groups: {totalGroups}";
            grid.View?.Refresh();
        }

        private void OnShowTotalOrders(TableSummaryContextInfo info)
        {
            if (info?.DataGrid == null) return;

            var grid = info.DataGrid;
            int totalOrders = grid.View?.Records?.Count ?? (grid.ItemsSource as System.Collections.ICollection)?.Count ?? 0;
            var row = info.SummaryRow ?? grid.TableSummaryRows.FirstOrDefault();

            if (row == null) return;

            row.ShowSummaryInRow = true;
            row.Title = $"Total Orders: {totalOrders}";
            grid.View?.Refresh();
        }

        private void OnCollapseGroup(GroupCaptionContextInfo info)
        {
            if (info?.DataGrid == null || info.Group == null) return;
            info.DataGrid.CollapseGroup(info.Group);
        }

        private void OnExpandGroup(GroupCaptionContextInfo info)
        {
            if (info?.DataGrid == null || info.Group == null) return;
            info.DataGrid.ExpandGroup(info.Group);
        }

        private void OnUndoDelete(RowContextMenuInfo info)
        {
            if (info?.DataGrid == null || _lastDeletedItem == null) return;

            var insertIndex = _lastDeletedIndex;
            if (insertIndex < 0 || insertIndex > OrderInfoCollection.Count)
            {
                insertIndex = OrderInfoCollection.Count;
            }

            OrderInfoCollection.Insert(insertIndex, _lastDeletedItem);
            _lastDeletedItem = null;
            _lastDeletedIndex = -1;
        }

        private void OnDelete(RowContextMenuInfo info)
        {
            if (info?.RowData is not OrderInfo row)
            {
                return;
            }

            _lastDeletedIndex = OrderInfoCollection.IndexOf(row);
            _lastDeletedItem = row;
            OrderInfoCollection.Remove(row);
        }

        private void OnClearSorting(HeaderContextInfo info)
        {
            info.DataGrid.SortColumnDescriptions.Clear();
        }

        private void OnSortDescending(HeaderContextInfo info)
        {
            if (info?.DataGrid == null || info?.Column == null) return;

            info.DataGrid.SortColumnDescriptions.Clear();
            info.DataGrid.SortColumnDescriptions.Add(new SortColumnDescription
            {
                ColumnName = info.Column.MappingName,
                SortDirection = ListSortDirection.Descending
            });
        }

        private void OnSortAscending(HeaderContextInfo info)
        {
            if (info?.DataGrid == null || info?.Column == null) return;

            info.DataGrid.SortColumnDescriptions.Clear();
            info.DataGrid.SortColumnDescriptions.Add(new SortColumnDescription
            {
                ColumnName = info.Column.MappingName,
                SortDirection = ListSortDirection.Ascending
            });
        }

        public void GenerateOrders()
        {
            orderInfo!.Add(new OrderInfo("1001", "MA0024", "Maria Anders", "Germany", "Berlin"));
            orderInfo.Add(new OrderInfo("1002", "AT0165", "Ana Trujillo", "Mexico", "Mexico D.F."));
            orderInfo.Add(new OrderInfo("1003", "AF0006", "Ant Fuller", "Mexico", "Mexico D.F."));
            orderInfo.Add(new OrderInfo("1004", "TH0280", "Thomas Hardy", "UK", "London"));
            orderInfo.Add(new OrderInfo("1005", "TA0058", "Tim Adams", "Sweden", "Gothenburg"));
            orderInfo.Add(new OrderInfo("1006", "HM0012", "Hanna Moos", "Germany", "Mannheim"));
            orderInfo.Add(new OrderInfo("1007", "AF0037", "Andrew Fuller", "France", "Strasbourg"));
            orderInfo.Add(new OrderInfo("1008", "MK0138", "Martin King", "Spain", "Madrid"));
            orderInfo.Add(new OrderInfo("1009", "LL0015", "Lenny Lin", "France", "Marseille"));
            orderInfo.Add(new OrderInfo("1010", "JC0126", "John Carter", "Canada", "Ottawa"));
            orderInfo.Add(new OrderInfo("1011", "LK0045", "Laura King", "UK", "London"));
            orderInfo.Add(new OrderInfo("1012", "AW0134", "Anne Wilson", "Germany", "Mannheim"));
            orderInfo.Add(new OrderInfo("1013", "MK0055", "Martin King", "France", "Strasbourg"));
            orderInfo.Add(new OrderInfo("1014", "GI0092", "Gina Irene", "UK", "London"));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
