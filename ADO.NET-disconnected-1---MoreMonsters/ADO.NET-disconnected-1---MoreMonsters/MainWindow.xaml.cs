using MonsterClassLibrary;
using System.Data;
using System.Reflection.Emit;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace ADO.NET_disconnected_1___MoreMonsters;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MonsterData.CreateMonsterDataTable();
        AllMonstersDataGrid.ItemsSource = MonsterData.MonsterDataTable.DefaultView;
        MonsterData.CreateFlyingMonsterDataTable();
        FlyingMonstersDataGrid.ItemsSource = MonsterData.FlyingMonsterDataTable.DefaultView;
        MonsterData.CreateLegendaryMonsterDataTable();
        LegendaryDataGrid.ItemsSource = MonsterData.LegendaryMonsterDataTable.DefaultView;
    }

    

    private void MonsterSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (((DataGrid)sender).SelectedItem is DataRowView row)
            DisplayMonsterDetails(row.Row);
    }

    private void DisplayMonsterDetails(DataRow monster)
    {
        MonsterNameTextBlock.Text = monster["name"].ToString();
        MonsterACTextBlock.Text = $"{monster["ac"]}";
        MonsterHPTextBlock.Text = $"{monster["hp"]}";
        MonsterSpeedTextBlock.Text = $"{monster["speed"]}";
        AlignmentTextBlock.Text = $"{monster["alignment"]}";
        SensesTextBlock.Text = monster["senses"].ToString();
        LanguagesTextBlock.Text = monster["languages"].ToString();
        MonsterSTRTextBlock.Text = monster["strength"].ToString();
        MonsterCHATextBlock.Text = monster["cha"].ToString();
        MonsterCONTextBlock.Text = monster["con"].ToString();
        MonsterINTTextBlock.Text = monster["intel"].ToString();
        MonsterWISTextBlock.Text = monster["wis"].ToString();
        MonsterDEXTextBlock.Text = monster["dex"].ToString();

        if (monster["image_source"] != DBNull.Value && !string.IsNullOrEmpty(monster["image_source"].ToString()))
        {
            MonsterImage.Visibility = Visibility.Visible;
            MonsterImage.Source = new BitmapImage(new Uri(monster["image_source"].ToString()));
        }
        else
        {
            MonsterImage.Visibility = Visibility.Hidden;
        }
    }

    
}