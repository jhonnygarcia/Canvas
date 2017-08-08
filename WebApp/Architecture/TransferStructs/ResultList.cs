using System;
using System.Collections.Generic;

namespace WebApp.Architecture.TransferStructs
{
    /// <summary>
    /// Result específico para Listas.
    /// </summary>
    /// <typeparam name="T">Tipo de los Elementos DTO a devolver</typeparam>
    public class ResultList<T> : BaseResult
        where T : class
    {
        private List<T> _elements;
        private bool _elementsSetted;

        private int _totalPages;

        /// <summary>
        /// Inicializa el Result con la lista Vacía.
        /// </summary>
        public ResultList()
        {
            PageCount = 0;
            _totalPages = 0;
            
            _elements = new List<T>();
            _elementsSetted = false;
        }

        /// <summary>
        /// Elementos del Result.
        /// </summary>
        public List<T> Elements {
            get { return _elements; }
            set
            {
                _elements = value;
                _elementsSetted = true;
            }
        }
        /// <summary>
        /// Total de Elementos en caso de ser paginado el resultado
        /// </summary>
        public int TotalElements { get; set; }
        /// <summary>
        /// Obtiene o establece el valor de a cantidad de elementos por página.
        /// Si esta propiedad se ha establecido, se utilizará para calcular el valor de TotalPages 
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// Total de Páginas en caso de ser paginado el resultado
        /// Si se ha establecido un valor en la propiedad <see cref="PageCount"/> este se utilizará para el cálculo del total de páginas
        /// </summary>
        public int TotalPages
        {
            get
            {
                if (PageCount <= 0) return _totalPages;

                if (_totalPages > 0)
                {
                    throw new InvalidOperationException("No derían establecerse valores en las propiedades PageCount y TotalPages a la vez.");
                }
                if (!_elementsSetted)
                {
                    throw new InvalidOperationException("Esta propiedad debe establecerse luego de dar valor a: 'Elements'");
                }

                var result = TotalElements / PageCount;
                if ((TotalElements % PageCount) > 0)
                    result++;

                return result;
            }
            set { _totalPages = value; }
        }
    }
}
