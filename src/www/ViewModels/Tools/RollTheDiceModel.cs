using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MawMvcApp.ViewModels.Tools;

public class RollTheDiceModel
{
    [Required(ErrorMessage = "Please enter the number of sides")]
    [Display(Name = "Number of Sides")]
    [Range(1, 20)]
    public int NumberOfSides { get; set; }

    [Required(ErrorMessage = "Please enter the number of throws")]
    [Display(Name = "Number of Throws")]
    [Range(1, 1000000)]
    public int NumberOfThrows { get; set; }

    [BindNever]
    public bool Executed { get; private set; }

    [BindNever]
    public List<int>? ThrowCounts { get; private set; }

    [BindNever]
    public bool HasErrors { get; set; }

    [BindNever]
    public IEnumerable<int>? WinnerList
    {
        get
        {
            if (ThrowCounts == null)
            {
                return null;
            }

            // so far have not gotten the linq to work...
            //return ThrowCounts.Select((idx, num) => idx + 1)
            //                  .Where((num, idx) => num == ThrowCounts.Max());

            int max = 0;
            var winnerList = new List<int>();

            for (int i = 0; i < NumberOfSides; i++)
            {
                if (ThrowCounts[i] > max)
                {
                    winnerList.Clear();
                    winnerList.Add(i + 1);
                    max = ThrowCounts[i];
                }
                else if (ThrowCounts[i] == max)
                {
                    winnerList.Add(i + 1);
                }
            }

            return winnerList;
        }
    }

    public void ThrowDice()
    {
        ThrowCounts = new List<int>(new int[NumberOfSides]);

        for (int i = 0; i < NumberOfThrows; i++)
        {
            ThrowCounts[RandomNumberGenerator.GetInt32(NumberOfSides)]++;
        }

        Executed = true;
    }
}
