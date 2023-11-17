using JMayer.Data.Data;
using System.Net;

namespace JMayer.Data.HTTP
{
    /// <summary>
    /// The class contains the results for the remote operation.
    /// </summary>
    public class OperationResult
    {
        /// <summary>
        /// The property gets/sets the data object returned by the operation.
        /// </summary>
        public DataObject? DataObject { get; private set; }

        /// <summary>
        /// The property gets if the operation was a success.
        /// </summary>
        public bool IsSuccessStatusCode { get => StatusCode == HttpStatusCode.OK; }

        /// <summary>
        /// The property gets/sets the HTTP status code returned by the remote operation.
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// The property constructor.
        /// </summary>
        /// <param name="dataObject">The data object returned by the operation.</param>
        /// <param name="httpStatusCode">The HTTP status code returned by the remote operation.</param>
        public OperationResult(DataObject? dataObject, HttpStatusCode httpStatusCode) 
        {
            DataObject = dataObject; 
            StatusCode = httpStatusCode;
        }
    }
}
