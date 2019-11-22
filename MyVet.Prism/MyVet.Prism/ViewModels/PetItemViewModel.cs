using MyVet.Common.Models;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyVet.Prism.ViewModels
{
    public class PetItemViewModel : PetResponse
    {
        private DelegateCommand _selectPetCommand;
    }
}
