using System;
using System.Collections.Generic;
using System.Text;

namespace MyVet.Common.Models
{
    public class Response<T> where T : class 
    {
        // esta calase me va a dar el estado de cdo yo consuma cualquiercosa en el api

        public bool IsSuccess { get; set; } // si pudo o no pudo

        public string Message { get; set; } // es la explicacion de porque no pudo...

        public T Result { get; set; } // es el resultado de la consulta...

    }
}
