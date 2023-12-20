using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace CMF
{
	//This script rotates a gameobject based on user input.
	//Rotation around the x-axis (vertical) can be clamped/limited by setting 'upperVerticalLimit' and 'lowerVerticalLimit'.
	public class CameraControllerPro : MonoBehaviour
    {
        public FP_Input mobile_Input;

        //public Transform PlayerTr;
        //public Transform VisualTr;
        //public GameObject VisualModel;

        public Transform CameraRoot;

        public CoinsManager coinsManager;
        public AudioEffects audioEffects;
        public GameObject playerModelVisual;
        public AdvancedWalkerController advancedWalkerController;
        //[HideInInspector] public AdvancedWalkerController advancedWalkerController;
        PlayerRespawn playerRespawn;
        float smoothX;

        //Current rotation values (in degrees);
        [HideInInspector] public float currentXAngle = 0f;
        [HideInInspector] public float currentYAngle = 0f;

		//Upper and lower limits (in degrees) for vertical rotation (along the local x-axis of the gameobject);
		[Range(0f, 90f)]
		public float upperVerticalLimit = 60f;
		[Range(0f, 90f)]
		public float lowerVerticalLimit = 60f;

        //Variables to store old rotation values for interpolation purposes;
        float oldHorizontalInput = 0f;
        float oldVerticalInput = 0f;

		//Camera turning speed; 
		public float cameraSpeed = 250f;

        public float smoothRot = 2f;

        //Whether camera rotation values will be smoothed;
        public bool smoothCameraRotation = false;

		//This value controls how smoothly the old camera rotation angles will be interpolated toward the new camera rotation angles;
		//Setting this value to '50f' (or above) will result in no smoothing at all;
		//Setting this value to '1f' (or below) will result in very noticable smoothing;
		//For most situations, a value of '25f' is recommended;
		[Range(1f, 50f)]
        public float cameraRotSmoothingFactor = 25f;
        [Range(1f, 50f)]
        public float cameraPosSmoothingFactor = 25f;

        //Variables for storing current facing direction and upwards direction;
        Vector3 facingDirection;
		Vector3 upwardsDirection;

		//References to transform and camera components;
		protected Transform tr;
		protected Camera cam;
        protected CameraInput cameraInput;
        protected CharacterInput characterInput;


        [Header("Bob Settings")]

        public float bobIntensity;
        public float bobIntensityX;
        public float bobSpeed;
        public float bobSmoothTransition;
        float bobSmooth;
        float SinTime;


        [Header("Chunks Placer")]

        public ChunksPlacer chunksPlacer;

        [Header("Interaction Settings")]

        public bool enableInteraction;

        public CanvasScaler canvasScaler;
        public LayerMask InteractionLayer;
        public LayerMask ContactLayer;
        public GameObject UI_hand;
        public GameObject UI_question;
        public GameObject UI_Key;

        Interaction nearestInteraction;
        Interaction lastNearestInteraction;
        Material lastInteractionMaterial;

        [HideInInspector] public float cameraHideOffset;

        bool haveKey;


        bool isInteractMobile;

        bool locked = false;


        //Setup references.
        void Awake () {
			tr = transform;
			cam = GetComponent<Camera>();
			cameraInput = GetComponent<CameraInput>();

            characterInput = advancedWalkerController.GetComponent<CharacterInput>();
            playerRespawn = advancedWalkerController.GetComponent<PlayerRespawn>();

            if (cameraInput == null)
				Debug.LogWarning("No camera input script has been attached to this gameobject", this.gameObject);

			//If no camera component has been attached to this gameobject, search the transform's children;
			if(cam == null)
				cam = GetComponentInChildren<Camera>();

            //Set angle variables to current rotation angles of this transform;
            currentXAngle = tr.localRotation.eulerAngles.x;
			currentYAngle = tr.localRotation.eulerAngles.y;

			//Execute camera rotation code once to calculate facing and upwards direction;
			RotateCamera(0f, 0f);

			Setup();
		}

		//This function is called right after Awake(); It can be overridden by inheriting scripts;
		protected virtual void Setup()
		{
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
		{
            if (locked)
                return;

			HandleCameraRotation();



            float tempMovX = advancedWalkerController.limitedInputX;
            float tempMovY = advancedWalkerController.limitedInputY;

            float tempBobSmooth = (tempMovX != 0 || tempMovY > 0) ? bobIntensity : (tempMovY < 0) ? bobIntensity * 0.75f : 0;
            bobSmooth = Mathf.Lerp(bobSmooth, tempBobSmooth, Time.deltaTime * bobSmoothTransition);

            SinTime += Time.deltaTime * bobSpeed * bobSmooth;


            float sinAmountY = -Mathf.Abs(Mathf.Sin(SinTime) * bobSmooth);
            float sinAmountX = Mathf.Sin(SinTime) * (bobIntensityX);

            if (playerRespawn.GameIsPaused == false && smoothRot != -1)
            {
                smoothX = Mathf.Clamp(Mathf.Lerp(smoothX, advancedWalkerController.limitedInputX * 1.5f + oldHorizontalInput * 11, Time.fixedDeltaTime * smoothRot), -40, 40);
                tr.localRotation *= Quaternion.Euler(new Vector3(sinAmountY, sinAmountX, smoothX / 5));
            }


            if (coinsManager)
            {
                Collider[] hitCoinColliders = Physics.OverlapSphere(tr.position, 1.5f, ContactLayer);
                foreach (var hitCollider in hitCoinColliders)
                {
                    if (hitCollider.CompareTag("Coinx1"))
                    {
                        hitCollider.enabled = false;
                        coinsManager.AddCoins(hitCollider.transform, 3);
                    }
                    else if (hitCollider.CompareTag("Coinx2"))
                    {
                        hitCollider.enabled = false;
                        coinsManager.AddCoins(hitCollider.transform, 6);
                    }
                    else if (hitCollider.CompareTag("Coinx3"))
                    {
                        hitCollider.enabled = false;
                        coinsManager.AddCoins(hitCollider.transform, 10);
                    }
                    else if (hitCollider.CompareTag("Key"))
                    {
                        haveKey = true;
                        hitCollider.enabled = false;
                        coinsManager.AddKey(hitCollider.transform, UI_Key);
                    }
                    else if (hitCollider.CompareTag("Door"))
                    {
                        OpenDoor(hitCollider.GetComponent<DoorElements>(), false);
                    }
                }
            }

           

            // Show Interaction

            if (enableInteraction == false) return;

            Vector3 spherePos = tr.position + (tr.forward * 0.75f);

            RaycastHit hit;
            if (Physics.Raycast(tr.position, tr.TransformDirection(Vector3.forward), out hit, 1.5f, InteractionLayer))
            {
                nearestInteraction = hit.collider.GetComponent<Interaction>();
                InteractionFollow(nearestInteraction.transform.position);
            }
            else
            {
                float minimumDistance = Mathf.Infinity;
                nearestInteraction = null;
                Collider[] hitColliders = Physics.OverlapSphere(spherePos, 0.75f, InteractionLayer);
                foreach (var hitCollider in hitColliders)
                {
                    float distance = Vector2.Distance(cam.WorldToViewportPoint(hitCollider.transform.position), new Vector2(0.5f, 0.5f));
                    if (distance < minimumDistance)
                    {
                        minimumDistance = distance;
                        nearestInteraction = hitCollider.GetComponent<Interaction>();
                    }
                }

                if (nearestInteraction)
                {
                    InteractionFollow(nearestInteraction.transform.position);
                }
                else if (UI_hand.activeSelf)
                {
                    UI_hand.SetActive(false);
                    if (lastInteractionMaterial)
                        lastInteractionMaterial.SetFloat("_ColorIntenseAdd", 0f);
                }
            }

            //
        }

        //Get user input and handle camera rotation;
        //This method can be overridden in classes derived from this base class to modify camera behaviour;
        protected virtual void HandleCameraRotation()
		{
			if(cameraInput == null)
				return;


            float _inputHorizontal = 0;
            float _inputVertical = 0;

            if (mobile_Input && playerRespawn.isMobile)
            {
                //_inputHorizontal = joystick.Horizontal;
                //_inputVertical = joystick.Vertical;

                _inputHorizontal = mobile_Input.LookInput().x * 0.25f;
                _inputVertical = mobile_Input.LookInput().y * 0.25f;
            }
            else
            {
                //Get input values;
                _inputHorizontal = cameraInput.GetHorizontalCameraInput();
                _inputVertical = cameraInput.GetVerticalCameraInput();
            }


            RotateCamera(_inputHorizontal, _inputVertical);
		}

		//Rotate camera; 
		protected void RotateCamera(float _newHorizontalInput, float _newVerticalInput)
		{
			if(smoothCameraRotation)
			{
				//Lerp input;
				oldHorizontalInput = Mathf.Lerp (oldHorizontalInput, _newHorizontalInput, Time.deltaTime * cameraRotSmoothingFactor);
				oldVerticalInput = Mathf.Lerp (oldVerticalInput, _newVerticalInput, Time.deltaTime * cameraRotSmoothingFactor);
			}
			else
			{
				//Replace old input directly;
				oldHorizontalInput = _newHorizontalInput;
				oldVerticalInput = _newVerticalInput;
			}

			//Add input to camera angles;
			currentXAngle += oldVerticalInput * cameraSpeed * Time.deltaTime;
			currentYAngle += oldHorizontalInput * cameraSpeed * Time.deltaTime;

			//Clamp vertical rotation;
			currentXAngle = Mathf.Clamp(currentXAngle, -upperVerticalLimit, lowerVerticalLimit);

            UpdateRotation();
		}

		//Update camera rotation based on x and y angles;
		protected void UpdateRotation()
		{

            //tr.localRotation = Quaternion.Euler(new Vector3(0, currentYAngle, 0));
            ////tr.localRotation = Quaternion.Euler(new Vector3(0, currentYAngle, 0));

            ////Save 'facingDirection' and 'upwardsDirection' for later;
            //facingDirection = tr.forward;
            //upwardsDirection = tr.up;

            //tr.localRotation = Quaternion.Euler(new Vector3(currentXAngle, 0, 0));

            ////tr.position = CamRootPoint.position;
            ////tr.position = Vector3.Lerp(tr.position, CamRootPoint.position, Time.deltaTime * cameraPosSmoothingFactor);

            //PlayerTr.rotation = Quaternion.Euler(new Vector3(0, currentYAngle, 0));
            //VisualTr.rotation = Quaternion.Euler(new Vector3(0, currentYAngle, 0));




            CameraRoot.localRotation = Quaternion.Euler(new Vector3(0, currentYAngle, 0));

            //Save 'facingDirection' and 'upwardsDirection' for later;
            facingDirection = CameraRoot.forward;
            upwardsDirection = CameraRoot.up;

            CameraRoot.localRotation = Quaternion.Euler(new Vector3(0, currentYAngle, 0));
            tr.localRotation = Quaternion.Euler(new Vector3(currentXAngle, 0, 0));

        }

		//Set the camera's field-of-view (FOV);
		public void SetFOV(float _fov)
		{
			if(cam)
				cam.fieldOfView = _fov;	
		}

		//Set x and y angle directly;
		public void SetRotationAngles(float _xAngle, float _yAngle)
		{
			currentXAngle = _xAngle;
			currentYAngle = _yAngle;

			UpdateRotation();
		}

        public void LookAtObject(Transform obj)
        {
            locked = true;
            if (playerModelVisual)
                playerModelVisual.SetActive(false);
            cam.transform.DOLookAt(obj.position, 1);
        }

        public void RotateZero()
        {
            cam.transform.DOLocalRotate(Vector3.zero, 1).OnComplete(Unlock);
            locked = false;
        }
        void Unlock()
        {
            if (playerModelVisual)
                playerModelVisual.SetActive(true);
            locked = false;
        }

        //Rotate the camera toward a rotation that points at a world position in the scene;
        public void RotateTowardPosition(Vector3 _position, float _lookSpeed)
		{
			//Calculate target look vector;
			Vector3 _direction = (_position - tr.position);

			RotateTowardDirection(_direction, _lookSpeed);
		}

		//Rotate the camera toward a look vector in the scene;
		public void RotateTowardDirection(Vector3 _direction, float _lookSpeed)
		{
			//Normalize direction;
			_direction.Normalize();

			//Transform target look vector to this transform's local space;
			_direction = tr.parent.InverseTransformDirection(_direction);

			//Calculate (local) current look vector; 
			Vector3 _currentLookVector = GetAimingDirection();
			_currentLookVector = tr.parent.InverseTransformDirection(_currentLookVector);

			//Calculate x angle difference;
			float _xAngleDifference = VectorMath.GetAngle(new Vector3(0f, _currentLookVector.y, 1f), new Vector3(0f, _direction.y, 1f), Vector3.right);

			//Calculate y angle difference;
			_currentLookVector.y = 0f;
			_direction.y = 0f;
			float _yAngleDifference = VectorMath.GetAngle(_currentLookVector, _direction, Vector3.up);

			//Turn angle values into Vector2 variables for better clamping;
			Vector2 _currentAngles = new Vector2(currentXAngle, currentYAngle);
			Vector2 _angleDifference = new Vector2(_xAngleDifference, _yAngleDifference);

			//Calculate normalized direction;
			float _angleDifferenceMagnitude = _angleDifference.magnitude;
			if(_angleDifferenceMagnitude == 0f)
				return;
			Vector2 _angleDifferenceDirection = _angleDifference/_angleDifferenceMagnitude;

			//Check for overshooting;
			if(_lookSpeed * Time.deltaTime > _angleDifferenceMagnitude)
			{
				_currentAngles += _angleDifferenceDirection * _angleDifferenceMagnitude;
			}
			else
				_currentAngles += _angleDifferenceDirection * _lookSpeed * Time.deltaTime;

			//Set new angles;
			currentYAngle = _currentAngles.y;
			//Clamp vertical rotation;
			currentXAngle = Mathf.Clamp(_currentAngles.x, -upperVerticalLimit, lowerVerticalLimit);
			
			UpdateRotation();
		}

		public float GetCurrentXAngle()
		{
			return currentXAngle;
		}

		public float GetCurrentYAngle()
		{
			return currentYAngle;
		}

		//Returns the direction the camera is facing, without any vertical rotation;
		//This vector should be used for movement-related purposes (e.g., moving forward);
		public Vector3 GetFacingDirection ()
		{
			return facingDirection;
		}

		//Returns the 'forward' vector of this gameobject;
		//This vector points in the direction the camera is "aiming" and could be used for instantiating projectiles or raycasts.
		public Vector3 GetAimingDirection ()
		{
			return tr.forward;
		}

		// Returns the 'right' vector of this gameobject;
		public Vector3 GetStrafeDirection ()
		{
			return tr.right;
		}

		// Returns the 'up' vector of this gameobject;
		public Vector3 GetUpDirection ()
		{
			return upwardsDirection;
		}



        public void OpenDoor(DoorElements doorElem, bool purchased)
        {
            if (doorElem.isKey)
            {
                if (!purchased)
                {
                    if (!(haveKey && UI_Key.activeSelf))
                    {
                        return;
                    }
                }
            }


            haveKey = false;
            doorElem.padlock.SetActive(false);
            UI_Key.SetActive(false);
            doorElem.gameObject.layer = 0;
            doorElem.GetComponent<Animator>().CrossFade("Door Open", 0.001f);
            chunksPlacer.GenChunk();

            if (audioEffects)
                audioEffects.PlaySound(audioEffects.doorOpen);
        }

        void InteractionFollow(Vector3 nearestInteractionPos)
        {

            if (nearestInteraction != lastNearestInteraction)
            {
                if (lastInteractionMaterial)
                    lastInteractionMaterial.SetFloat("_ColorIntenseAdd", 0f);

                lastNearestInteraction = nearestInteraction;

                if (lastNearestInteraction.materialNumber >= 0)
                    lastInteractionMaterial = lastNearestInteraction.GetComponent<MeshRenderer>().materials[lastNearestInteraction.materialNumber];
                else
                    lastInteractionMaterial = null;
            }

            if (lastNearestInteraction.isAnimate || (lastNearestInteraction.isBuy && haveKey))
            {
                if (UI_hand.activeSelf)
                    UI_hand.SetActive(false);

                return;
            }

            if (lastNearestInteraction.isHide || lastNearestInteraction.isUsed)
            {
                if (UI_question.activeSelf)
                    UI_question.SetActive(false);
            }
            else
            {
                if (!UI_question.activeSelf)
                    UI_question.SetActive(true);
            }

            Vector2 viewportPosition = cam.WorldToViewportPoint(nearestInteractionPos);
            Vector2 finalPosition = new Vector2(viewportPosition.x * canvasScaler.referenceResolution.x - (canvasScaler.referenceResolution.x * 0.5f), viewportPosition.y * canvasScaler.referenceResolution.y - (canvasScaler.referenceResolution.y * 0.5f));

            UI_hand.GetComponent<RectTransform>().anchoredPosition = finalPosition;
            UI_hand.SetActive(true);

            if (lastInteractionMaterial)
                lastInteractionMaterial.SetFloat("_ColorIntenseAdd", 1f);

            if (characterInput.IsInteractKeyPressed() || isInteractMobile)
            {
                isInteractMobile = false;
                //lastNearestInteraction.Interact(this, advancedWalkerController);
            }
        }

        public void InteractMobile()
        {
            isInteractMobile = true;
            Invoke("DisableInteractMobile", 0.2f);
        }

        void DisableInteractMobile()
        {
            if (isInteractMobile)
                isInteractMobile = false;
        }
    }
}
