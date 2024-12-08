using UnityEngine;

public interface ISourceProvider<T> where T : MonoBehaviour
{
    T dataSourceRef { get; set; }
}