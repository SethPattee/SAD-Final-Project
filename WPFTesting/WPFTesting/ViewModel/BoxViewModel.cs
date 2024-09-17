using System.Collections.ObjectModel;
using WPFTesting.Data;
using WPFTesting.Shapes;

namespace WPFTesting.ViewModel;

public class BoxViewModel
{
    private IBoxDataProvider _boxProvider;
    public BoxViewModel(IBoxDataProvider boxProvider)
    {
        _boxProvider = boxProvider;
    }
    public ObservableCollection<BoxValues> Boxes { get; set; } = new();

    public async Task LoadAsync()
    {
        //if (Boxes.Any()) return; //don't overide any existing data

        var boxes = await _boxProvider.GetBoxValuesAsync();
        if (boxes is not null)
        {
            foreach (var box in boxes)
            {
                Boxes.Add(box);
            }
        }

    }
}
