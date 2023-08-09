using System;

namespace Blog.ViewModels           
{
    public class ResultViewModel <T> // classe generica 
    {
        public ResultViewModel(T? data, List <string>? errors) // padronização de erros
        {
            Data = data;
            Errors = errors;            
        }

        public ResultViewModel(T? data) // em caso de sucesso, só recebe o dado
        {
            Data = data;
            
        }

        public ResultViewModel(List<string>? errors) // só recebo uma lista de erros
        {
            Errors = errors;
            
        }

        public ResultViewModel(string error)
        {
            Errors.Add(error);
        }
        public T? Data { get; private set; }         //private impede q ele seja alterado

        public List<string>? Errors { get; private set; } = new();// inicializando a lista

    }



}
