using UnityEngine;

public static class Utilities
{
    #region Object Placement

    /// <summary>
    /// Used to position Objects relative to the Screen. Top Left, Top Center, Top Right Middle
    /// Left, Middle Center, Middle Right Bottom Left, Bottom Center, Bottom Right
    /// </summary>
    /// <param name="place">The 9 points on the screen.</param>
    /// <param name="rect">The object you want to place</param>
    /// <returns>Returns Rect place in screen location</returns>
    public static Rect PlaceToScreen (PlaceObject place, Rect rect)
    {
        switch (place)
        {
            case PlaceObject.TopLeft:
                rect = new Rect(0, 0, rect.width, rect.height);
                break;

            case PlaceObject.TopCenter:
                rect = new Rect((Screen.width - rect.width) / 2, 0, rect.width, rect.height);
                break;

            case PlaceObject.TopRight:
                rect = new Rect(Screen.width - rect.width, 0, rect.width, rect.height);
                break;

            case PlaceObject.MiddleLeft:
                rect = new Rect(0, (Screen.height - rect.width) / 2, rect.width, rect.height);
                break;

            case PlaceObject.MiddleCenter:
                rect = new Rect((Screen.width - rect.width) / 2, (Screen.height - rect.height) / 2, rect.width, rect.height);
                break;

            case PlaceObject.MiddleRight:
                rect = new Rect(Screen.width - rect.width, (Screen.height - rect.height) / 2, rect.width, rect.height);
                break;

            case PlaceObject.BottomLeft:
                rect = new Rect(0, Screen.height - rect.height, rect.width, rect.height);
                break;

            case PlaceObject.BottomCenter:
                rect = new Rect((Screen.width - rect.width) / 2, Screen.height - rect.height, rect.width, rect.height);
                break;

            case PlaceObject.BottomRight:
                rect = new Rect(Screen.width - rect.width, Screen.height - rect.height, rect.width, rect.height);
                break;
        }

        return rect;
    }

    /// <summary>
    /// Used to position Objects relative to the Screen. Top Left, Top Center, Top Right Middle
    /// Left, Middle Center, Middle Right Bottom Left, Bottom Center, Bottom Right
    /// </summary>
    /// <param name="place">The 9 points on the screen.</param>
    /// <param name="rect">The object you want to place</param>
    /// <param name="horizontalPadding">
    /// Padding will be applied to the LEFT if the VALUE is POSITIVE, and RIGHT if VALUE is NEGATIVE
    /// </param>
    /// <param name="verticalPadding">
    /// Padding will be applied to the TOP if the VALUE is POSITIVE, and BOTTOM if VALUE is NEGATIVE
    /// </param>
    /// <returns>Returns Rect place in screen location with padding</returns>
    public static Rect PlaceToScreen (PlaceObject place, Rect rect, float horizontalPadding, float verticalPadding)
    {
        switch (place)
        {
            case PlaceObject.TopLeft:
                rect = new Rect(0, 0, rect.width, rect.height);
                break;

            case PlaceObject.TopCenter:
                rect = new Rect((Screen.width - rect.width) / 2, 0, rect.width, rect.height);
                break;

            case PlaceObject.TopRight:
                rect = new Rect(Screen.width - rect.width, 0, rect.width, rect.height);
                break;

            case PlaceObject.MiddleLeft:
                rect = new Rect(0, (Screen.height - rect.width) / 2, rect.width, rect.height);
                break;

            case PlaceObject.MiddleCenter:
                rect = new Rect((Screen.width - rect.width) / 2, (Screen.height - rect.height) / 2, rect.width, rect.height);
                break;

            case PlaceObject.MiddleRight:
                rect = new Rect(Screen.width - rect.width, (Screen.height - rect.height) / 2, rect.width, rect.height);
                break;

            case PlaceObject.BottomLeft:
                rect = new Rect(0, Screen.height - rect.height, rect.width, rect.height);
                break;

            case PlaceObject.BottomCenter:
                rect = new Rect((Screen.width - rect.width) / 2, Screen.height - rect.height, rect.width, rect.height);
                break;

            case PlaceObject.BottomRight:
                rect = new Rect(Screen.width - rect.width, Screen.height - rect.height, rect.width, rect.height);
                break;
        }

        return rect = new Rect(rect.x + horizontalPadding, rect.y + verticalPadding, rect.width, rect.height);
        ;
    }

    /// <summary>
    /// Used to position Objects relative the parent object. Top Left, Top Center, Top Right Middle
    /// Left, Middle Center, Middle Right Bottom Left, Bottom Center, Bottom Right
    /// </summary>
    /// <param name="place">The 9 points on the screen.</param>
    /// <param name="parentRect">The parent rect of the object that is being placed</param>
    /// <param name="childRect">The child rect of the parent rect thats is being placed</param>
    /// <returns>Return the location of the child object relative to the parent object.</returns>
    public static Rect PlaceToObject (PlaceObject place, Rect parentRect, Rect childRect)
    {
        Rect rect = new Rect(0, 0, 0, 0);

        switch (place)
        {
            case PlaceObject.TopLeft:
                rect = new Rect(parentRect.x - childRect.x, parentRect.y - childRect.y, childRect.width, childRect.height);
                break;

            case PlaceObject.TopCenter:
                rect = new Rect((parentRect.x - childRect.x) + (parentRect.width - childRect.width) / 2, parentRect.y - childRect.y, childRect.width, childRect.height);
                break;

            case PlaceObject.TopRight:
                rect = new Rect((parentRect.x - childRect.x) + (parentRect.width - childRect.width), parentRect.y - childRect.y, childRect.width, childRect.height);
                break;

            case PlaceObject.MiddleLeft:
                rect = new Rect(parentRect.x - childRect.x, (parentRect.y - childRect.y) + (parentRect.height - childRect.height) / 2, childRect.width, childRect.height);
                break;

            case PlaceObject.MiddleCenter:
                rect = new Rect((parentRect.x - childRect.x) + (parentRect.width - childRect.width) / 2, (parentRect.y - childRect.y) + (parentRect.height - childRect.height) / 2, childRect.width, childRect.height);
                break;

            case PlaceObject.MiddleRight:
                rect = new Rect((parentRect.x - childRect.x) + parentRect.width - childRect.width, (parentRect.y - childRect.y) + (parentRect.height - childRect.height) / 2, childRect.width, childRect.height);
                break;

            case PlaceObject.BottomLeft:
                rect = new Rect(parentRect.x - childRect.x, (parentRect.y - childRect.y) + parentRect.height - childRect.height, childRect.width, childRect.height);
                break;

            case PlaceObject.BottomCenter:
                rect = new Rect((parentRect.x - childRect.x) + (parentRect.width - childRect.width) / 2, (parentRect.y - childRect.y) + parentRect.height - childRect.height, childRect.width, childRect.height);
                break;

            case PlaceObject.BottomRight:
                rect = new Rect((parentRect.x - childRect.x) + (parentRect.width - childRect.width), (parentRect.y - childRect.y) + parentRect.height - childRect.height, childRect.width, childRect.height);
                break;
        }

        return rect;
    }

    /// <summary>
    /// Used to position Objects relative the parent object with padding adjustment Top Left, Top
    /// Center, Top Right Middle Left, Middle Center, Middle Right Bottom Left, Bottom Center,
    /// Bottom Right
    /// </summary>
    /// <param name="place"></param>
    /// <param name="parentRect">The parent rect of the object that is being placed</param>
    /// <param name="childRect">The child rect of the parent rect thats is being placed</param>
    /// <param name="horizontalPadding">
    /// Padding will be applied to the LEFT if the VALUE is POSITIVE, and RIGHT if VALUE is NEGATIVE
    /// </param>
    /// <param name="verticalPadding">
    /// Padding will be applied to the TOP if the VALUE is POSITIVE, and BOTTOM if VALUE is NEGATIVE
    /// </param>
    /// <returns>Return the location of the child object relative to the parent object.</returns>
    public static Rect PlaceToObject (PlaceObject place, Rect parentRect, Rect childRect, float horizontalPadding, float verticalPadding)
    {
        Rect rect = new Rect(0, 0, 0, 0);

        switch (place)
        {
            case PlaceObject.TopLeft:
                rect = new Rect(parentRect.x - childRect.x, parentRect.y - childRect.y, childRect.width, childRect.height);
                break;

            case PlaceObject.TopCenter:
                rect = new Rect((parentRect.x - childRect.x) + (parentRect.width - childRect.width) / 2, parentRect.y - childRect.y, childRect.width, childRect.height);
                break;

            case PlaceObject.TopRight:
                rect = new Rect((parentRect.x - childRect.x) + (parentRect.width - childRect.width), parentRect.y - childRect.y, childRect.width, childRect.height);
                break;

            case PlaceObject.MiddleLeft:
                rect = new Rect(parentRect.x - childRect.x, (parentRect.y - childRect.y) + (parentRect.height - childRect.height) / 2, childRect.width, childRect.height);
                break;

            case PlaceObject.MiddleCenter:
                rect = new Rect((parentRect.x - childRect.x) + (parentRect.width - childRect.width) / 2, (parentRect.y - childRect.y) + (parentRect.height - childRect.height) / 2, childRect.width, childRect.height);
                break;

            case PlaceObject.MiddleRight:
                rect = new Rect((parentRect.x - childRect.x) + parentRect.width - childRect.width, (parentRect.y - childRect.y) + (parentRect.height - childRect.height) / 2, childRect.width, childRect.height);
                break;

            case PlaceObject.BottomLeft:
                rect = new Rect(parentRect.x - childRect.x, (parentRect.y - childRect.y) + parentRect.height - childRect.height, childRect.width, childRect.height);
                break;

            case PlaceObject.BottomCenter:
                rect = new Rect((parentRect.x - childRect.x) + (parentRect.width - childRect.width) / 2, (parentRect.y - childRect.y) + parentRect.height - childRect.height, childRect.width, childRect.height);
                break;

            case PlaceObject.BottomRight:
                rect = new Rect((parentRect.x - childRect.x) + (parentRect.width - childRect.width), (parentRect.y - childRect.y) + parentRect.height - childRect.height, childRect.width, childRect.height);
                break;
        }

        return rect = new Rect(rect.x + horizontalPadding, rect.y + verticalPadding, rect.width, rect.height);
    }

    #endregion Object Placement

    public static Vector3 GetMeshWidthHeight (Mesh mesh)
    {
        Vector3 pos = Vector3.zero;
        Vector3 posStart = Camera.main.WorldToScreenPoint(new Vector3(mesh.bounds.min.x, mesh.bounds.min.y, mesh.bounds.min.z));
        Vector3 posEnd = Camera.main.WorldToScreenPoint(new Vector3(mesh.bounds.max.x, mesh.bounds.max.y, mesh.bounds.min.z));

        int widthX = (int)(posEnd.x - posStart.x); // 50
        int widthY = (int)(posEnd.y - posStart.y); // 50

        pos.x = widthX;
        pos.y = widthX;

        return pos;
    }

    public static Vector3 GetColliderWidthHeight (Collider collider)
    {
        Vector3 pos = Vector3.zero;
        Vector3 posStart = Camera.main.WorldToScreenPoint(new Vector3(collider.bounds.min.x, collider.bounds.min.y, collider.bounds.min.z));
        Vector3 posEnd = Camera.main.WorldToScreenPoint(new Vector3(collider.bounds.max.x, collider.bounds.max.y, collider.bounds.min.z));

        int widthX = (int)(posEnd.x - posStart.x); // 50
        int widthY = (int)(posEnd.y - posStart.y); // 50

        pos.x = widthX;
        pos.y = widthX;

        return pos;
    }

    public static Vector3 GetCollider2DWidthHeight (Collider2D collider)
    {
        Vector3 pos = Vector3.zero;
        Vector3 posStart = Camera.main.WorldToScreenPoint(new Vector3(collider.bounds.min.x, collider.bounds.min.y, collider.bounds.min.z));
        Vector3 posEnd = Camera.main.WorldToScreenPoint(new Vector3(collider.bounds.max.x, collider.bounds.max.y, collider.bounds.min.z));

        int widthX = (int)(posEnd.x - posStart.x); // 50
        int widthY = (int)(posEnd.y - posStart.y); // 50

        pos.x = widthX;
        pos.y = widthX;

        return pos;
    }
}

public enum PlaceObject
{
    TopLeft,
    TopCenter,
    TopRight,
    MiddleLeft,
    MiddleCenter,
    MiddleRight,
    BottomLeft,
    BottomCenter,
    BottomRight
}