using System;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// unitytips: ShadowCaster2DFromCollider Component
/// http://diegogiacomelli.com.br/unitytips-shadowcaster2-from-collider-component
/// <remarks>
/// Based on https://forum.unity.com/threads/can-2d-shadow-caster-use-current-sprite-silhouette.861256/
/// </remarks>
/// </summary>
[RequireComponent(typeof(ShadowCaster2D))]
[DefaultExecutionOrder(100)]
public class ShadowCaster2DFromCollider : MonoBehaviour
{

    static readonly FieldInfo _meshField;
    static readonly FieldInfo _shapePathField;
    static readonly MethodInfo _generateShadowMeshMethod;

    ShadowCaster2D _shadowCaster;

    EdgeCollider2D _edgeCollider;
    PolygonCollider2D _polygonCollider;

    static ShadowCaster2DFromCollider()
    {
        _meshField = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);
        _shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);

        _generateShadowMeshMethod = typeof(ShadowCaster2D)
                                    .Assembly
                                    .GetType("UnityEngine.Experimental.Rendering.Universal.ShadowUtils")
                                    .GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static);
    }

    private void Start()
    {
        _shadowCaster = this.GetComponent<ShadowCaster2D>();
        _edgeCollider = this.GetComponent<EdgeCollider2D>();

        if (_edgeCollider == null)
            _polygonCollider = this.GetComponent<PolygonCollider2D>();

        UpdateShadow();
    }

    public void UpdateShadow()
    {
        var points = _polygonCollider == null
            ? _edgeCollider.points
            : _polygonCollider.points;


        Vector3[] points3 = new Vector3[points.Length];
        foreach (Vector2 p in points)
        {
            points3[Array.IndexOf(points,p)] = new(p.x,p.y,0);
        }

        _shapePathField.SetValue(_shadowCaster, points3);
        _meshField.SetValue(_shadowCaster, new Mesh());
        _generateShadowMeshMethod.Invoke(_shadowCaster, new object[] { _meshField.GetValue(_shadowCaster), _shapePathField.GetValue(_shadowCaster) });
    }
}
