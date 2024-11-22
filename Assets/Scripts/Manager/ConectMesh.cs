using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ConectMesh : MonoBehaviour
{
    void Start()
    {
        CombineMeshes();
        AddBoxCollider();
    }

    void CombineMeshes()
    {
        // Obtener todos los MeshFilter de los hijos
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        // Recorrer todos los MeshFilters para combinarlos
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false); // Desactivar el objeto original
        }

        // Crear un nuevo Mesh para el objeto actual
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.CombineMeshes(combine, true, true);

        // Activar el nuevo mesh combinado y destruir los hijos
        gameObject.SetActive(true);
    }

    void AddBoxCollider()
    {
        // Si ya hay un BoxCollider, lo destruye para agregar uno nuevo
        BoxCollider existingCollider = GetComponent<BoxCollider>();
        if (existingCollider != null)
        {
            Destroy(existingCollider);
        }

        // Añadir un nuevo BoxCollider que se ajuste al mesh combinado
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.center = GetComponent<MeshRenderer>().bounds.center;
        Vector3 colliderSize = GetComponent<MeshRenderer>().bounds.size;

        // Configurar el tamaño en Y del BoxCollider a 0
        colliderSize.y = 0;
        boxCollider.size = colliderSize;
    }
}
