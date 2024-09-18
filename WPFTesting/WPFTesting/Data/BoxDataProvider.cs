using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTesting.Shapes;

namespace WPFTesting.Data;

public interface IBoxDataProvider
{
    Task<IEnumerable<BoxValues>?> GetBoxValuesAsync();
}
public class BoxDataProvider : IBoxDataProvider
{
    public async Task<IEnumerable<BoxValues>?> GetBoxValuesAsync()
    {
        await Task.Delay(0);
        return new List<BoxValues>
        {
            new BoxValues { Items= new List<string>{"first item", "second item"},
                Title="Box-Title",
                xPosition=140,
                yPosition=140
            },
            new BoxValues
            {
                Items= new List<string>{"first", "second"},
                Title="my title",
                xPosition=280,
                yPosition=280
            },
            new BoxValues
            {
                Items= new List<string>{"screws", "nails", "2x4s", "fourth item" },
                Title="lost Hardware",
                xPosition=50,
                yPosition=320
            }
        };
    }
}
