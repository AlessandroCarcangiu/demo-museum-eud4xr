using ECARules4All_DLL.Utils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


namespace ECARules4All_DLL.Taxonomies.Behaviours.Subcategories
{
    [ECARules4All("ecasocket")]
    [DisallowMultipleComponent]
    public class ECASocket : MonoBehaviour
    {
        private XRSocketInteractor _socketInteractor;
        public ECAOutline previewOutline;

        private void Awake()
        {
            _socketInteractor = GetComponent<XRSocketInteractor>();
            if (_socketInteractor == null)
            {
                Debug.LogError("No XRSocketInteractor found on " + gameObject.name);
            }

            if (previewOutline == null)
            {
                Debug.LogWarning("[WARNING] No ECAOutline found on " + gameObject.name);
            }

            _socketInteractor.selectEntered.AddListener(OnObjectSet);
            _socketInteractor.selectExited.AddListener(OnObjectUnset);
        }

        [StateVariable("content", ECARules4AllType.Text)]
        [ECARelevance(true)]
        public string content
        {
            get => _content;
            set
            {
                _content = value;
                ECAScript.NotifyUpdate(this, nameof(content), content);
            }
        }

        private string _content;

        private void OnObjectSet(SelectEnterEventArgs args)
        {
            var gO = args.interactableObject.transform.gameObject;
            ECAObject gO_ECAObject = gO.GetComponent<ECAObject>();
            if (gO_ECAObject == null)
            {
                return;
            }

            content = gO.name;
            if (previewOutline) previewOutline.enabled = false;
        }

        private void OnObjectUnset(SelectExitEventArgs args)
        {
            var gO = args.interactableObject.transform.gameObject;
            ECAObject gO_ECAObject = gO.GetComponent<ECAObject>();
            if (gO_ECAObject == null)
            {
                return;
            }

            content = string.Empty;
            if (previewOutline) previewOutline.enabled = true;
        }
    }
}