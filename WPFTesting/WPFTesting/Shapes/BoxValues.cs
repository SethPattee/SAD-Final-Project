using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTesting.Shapes;

public class BoxValues
{
    public int xPosition {  get; set; }
    public int yPosition {get; set;}
    public string? Title { get; set; }
    public List<string>? Items { get; set; }
    //ToDo: add unique Id for lines to connect to
    //ToDo: add list of boxIds that we connect lines to
    //ToDo: turn into a parent child class for differnet kinds of boxes
}
