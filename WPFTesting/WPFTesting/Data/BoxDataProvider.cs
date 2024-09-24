using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFTesting.Shapes;
using WPFTesting.Models;

namespace WPFTesting.Data;


public class BoxDataProvider : IBoxDataProvider
{
    public async Task<IEnumerable<BoxValues>?> GetBoxValuesAsync()
    {
        await Task.Delay(0);
        return new List<BoxValues>
        {
            new BoxValues { 
                supplier = new Supplier()
                {

                    Name="Box-Title",
                },
                xPosition=140,
                yPosition=140
            },
            new BoxValues
            {
                supplier = new Supplier()
                {

                    Name="my company",
                },
                xPosition=280,
                yPosition=280
            },
            new BoxValues
            {
                supplier = new Supplier()
                {
                    Products =
                    {
                        new Product()
                        {
                            Quantity = 1,
                            ProductName = "Hammer"
                        },
                        new Product()
                        {
                            Quantity=1,
                            ProductName="10mm socket"
                        },
                        new Product()
                        {
                            Quantity=20,
                            ProductName="screws"
                        },
                        new Product()
                        {
                            Quantity=1000,
                            ProductName="sawdust",
                            Units="kg"
                        }
                    },
                    Name="Lost Hardware",
                },
                xPosition=50,
                yPosition=320
            }
        };
    }

    public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
    {
        await Task.Delay(100);

        return new List<Supplier> { };
    }


}
