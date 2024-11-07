using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace PassKeeper.Helpers
{
    public static class Icons
    {
        public static ObservableCollection<SymbolIcon> IconOptions { get; } = new ObservableCollection<SymbolIcon>
        {
            new SymbolIcon { Name = "Key16", Symbol = SymbolRegular.Key16, Uid = "0" },
            new SymbolIcon { Name = "Person16", Symbol = SymbolRegular.Person16, Uid = "1" },
            new SymbolIcon { Name = "Heart12", Symbol = SymbolRegular.Heart12, Uid = "2" },
            new SymbolIcon { Name = "BuildingBank16", Symbol = SymbolRegular.BuildingBank16, Uid = "3" },
            new SymbolIcon { Name = "Games16", Symbol = SymbolRegular.Games16, Uid = "4" },
            new SymbolIcon { Name = "Important12", Symbol = SymbolRegular.Important12, Uid = "5" },
            new SymbolIcon { Name = "Folder16", Symbol = SymbolRegular.Folder16, Uid = "6" }
        };

    }
}
