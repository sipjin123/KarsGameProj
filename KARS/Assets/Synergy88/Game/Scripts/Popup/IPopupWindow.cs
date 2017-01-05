using System.Collections;

using uPromise;

namespace Synergy88
{
    public interface IPopupWindow
    {
        Promise In();
        Promise Out();
    }
}