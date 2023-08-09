using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace Blog.Extensions   // padrão: classes de extensão precisam ser estaticas
{
    public static class ModelStateExtension
    {   
        // this: adiciona a metodo GetErrors em todos os ModelStates ||  isto é o q torna o GetErrors uma extensão
        public static List<string> GetErrors(this ModelStateDictionary modelState)
        {
            var result = new List <string>();
            foreach (var item in modelState.Values)
                result.AddRange(item.Errors.Select(errors => errors.ErrorMessage));

            return result;
        }

    }
}

