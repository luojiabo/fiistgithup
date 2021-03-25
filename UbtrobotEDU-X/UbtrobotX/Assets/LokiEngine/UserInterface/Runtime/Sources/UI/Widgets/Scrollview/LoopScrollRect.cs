using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;

namespace Loki.UI
{
	[AddComponentMenu("")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	public abstract class LoopScrollRect : UIBehaviour,
		IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, ICanvasElement, ILayoutElement, ILayoutGroup
	{
		//==========LoopScrollRect==========
		[Tooltip("Prefab Source")]
		public LoopScrollPrefabSource prefabSource;

		[Tooltip("Total count, negative means INFINITE mode")]
		public int totalCount;

		[HideInInspector]
		[NonSerialized]
		public LoopScrollDataSource dataSource = LoopScrollSendIndexSource.Instance;

		protected float threshold = 0;

		[Tooltip("Reverse direction for dragging")]
		public bool reverseDirection = false;

		[Tooltip("Rubber scale for outside")]
		public float rubberScale = 1;

		protected int itemTypeStart = 0;
		protected int itemTypeEnd = 0;

		protected abstract float GetSize(RectTransform item);
		protected abstract float GetDimension(Vector2 vector);
		protected abstract Vector2 GetVector(float value);
		protected int directionSign = 0;

		private float mContentSpacing = -1;
		protected GridLayoutGroup mGridLayout = null;
		protected float contentSpacing
		{
			get
			{
				if (mContentSpacing >= 0)
				{
					return mContentSpacing;
				}
				mContentSpacing = 0;
				if (content != null)
				{
					HorizontalOrVerticalLayoutGroup layout1 = content.GetComponent<HorizontalOrVerticalLayoutGroup>();
					if (layout1 != null)
					{
						mContentSpacing = layout1.spacing;
					}
					mGridLayout = content.GetComponent<GridLayoutGroup>();
					if (mGridLayout != null)
					{
						mContentSpacing = Mathf.Abs(GetDimension(mGridLayout.spacing));
					}
				}
				return mContentSpacing;
			}
		}

		private int mContentConstraintCount = 0;
		protected int contentConstraintCount
		{
			get
			{
				if (mContentConstraintCount > 0)
				{
					return mContentConstraintCount;
				}
				mContentConstraintCount = 1;
				if (content != null)
				{
					GridLayoutGroup layout2 = content.GetComponent<GridLayoutGroup>();
					if (layout2 != null)
					{
						if (layout2.constraint == GridLayoutGroup.Constraint.Flexible)
						{
							DebugUtility.LogWarning(LoggerTags.UI, "[LoopScrollRect] Flexible not supported yet");
						}
						mContentConstraintCount = layout2.constraintCount;
					}
				}
				return mContentConstraintCount;
			}
		}

		/// <summary>
		/// the first line
		/// </summary>
		int startLine
		{
			get
			{
				return Mathf.CeilToInt((float)(itemTypeStart) / contentConstraintCount);
			}
		}

		/// <summary>
		/// how many lines we have for now
		/// </summary>
		public int currentLines
		{
			get
			{
				return Mathf.CeilToInt((float)(itemTypeEnd - itemTypeStart) / contentConstraintCount);
			}
		}

		/// <summary>
		/// how many lines we have in total
		/// </summary>
		public int totalLines
		{
			get
			{
				return Mathf.CeilToInt((float)(totalCount) / contentConstraintCount);
			}
		}

		public object[] objectsToFill
		{
			// wrapper for forward compatibility
			set
			{
				if (value != null)
					dataSource = new LoopScrollArraySource<object>(value);
				else
					dataSource = LoopScrollSendIndexSource.Instance;
			}
		}

		protected virtual bool UpdateItems(Bounds viewBounds, Bounds contentBounds) { return false; }
		//==========LoopScrollRect==========

		public enum EMovementType
		{
			Unrestricted, // Unrestricted movement -- can scroll forever
			Elastic, // Restricted but flexible -- can go past the edges, but springs back in place
			Clamped, // Restricted movement where it's not possible to go past the edges
		}

		public enum EScrollbarVisibility
		{
			Permanent,
			AutoHide,
			AutoHideAndExpandViewport,
		}

		[Serializable]
		public class ScrollRectEvent : UnityEvent<Vector2> { }

		[SerializeField]
		private RectTransform mContent;
		public RectTransform content { get { return mContent; } set { mContent = value; } }

		[SerializeField]
		private bool mHorizontal = true;
		public bool horizontal { get { return mHorizontal; } set { mHorizontal = value; } }

		[SerializeField]
		private bool mVertical = true;
		public bool vertical { get { return mVertical; } set { mVertical = value; } }

		[SerializeField]
		private EMovementType mMovementType = EMovementType.Elastic;
		public EMovementType movementType { get { return mMovementType; } set { mMovementType = value; } }

		[SerializeField]
		private float mElasticity = 0.1f; // Only used for MovementType.Elastic
		public float elasticity { get { return mElasticity; } set { mElasticity = value; } }

		[SerializeField]
		private bool mInertia = true;
		public bool inertia { get { return mInertia; } set { mInertia = value; } }

		[SerializeField]
		private float mDecelerationRate = 0.135f; // Only used when inertia is enabled
		public float decelerationRate { get { return mDecelerationRate; } set { mDecelerationRate = value; } }

		[SerializeField]
		private float mScrollSensitivity = 1.0f;
		public float scrollSensitivity { get { return mScrollSensitivity; } set { mScrollSensitivity = value; } }

		[SerializeField]
		private RectTransform mViewport;
		public RectTransform viewport { get { return mViewport; } set { mViewport = value; SetDirtyCaching(); } }

		[SerializeField]
		private Scrollbar mHorizontalScrollbar;
		public Scrollbar horizontalScrollbar
		{
			get
			{
				return mHorizontalScrollbar;
			}
			set
			{
				if (mHorizontalScrollbar)
					mHorizontalScrollbar.onValueChanged.RemoveListener(SetHorizontalNormalizedPosition);
				mHorizontalScrollbar = value;
				if (mHorizontalScrollbar)
					mHorizontalScrollbar.onValueChanged.AddListener(SetHorizontalNormalizedPosition);
				SetDirtyCaching();
			}
		}

		[SerializeField]
		private Scrollbar mVerticalScrollbar;
		public Scrollbar verticalScrollbar
		{
			get
			{
				return mVerticalScrollbar;
			}
			set
			{
				if (mVerticalScrollbar)
					mVerticalScrollbar.onValueChanged.RemoveListener(SetVerticalNormalizedPosition);
				mVerticalScrollbar = value;
				if (mVerticalScrollbar)
					mVerticalScrollbar.onValueChanged.AddListener(SetVerticalNormalizedPosition);
				SetDirtyCaching();
			}
		}

		[SerializeField]
		private EScrollbarVisibility mHorizontalScrollbarVisibility;
		public EScrollbarVisibility horizontalScrollbarVisibility { get { return mHorizontalScrollbarVisibility; } set { mHorizontalScrollbarVisibility = value; SetDirtyCaching(); } }

		[SerializeField]
		private EScrollbarVisibility mVerticalScrollbarVisibility;
		public EScrollbarVisibility verticalScrollbarVisibility { get { return mVerticalScrollbarVisibility; } set { mVerticalScrollbarVisibility = value; SetDirtyCaching(); } }

		[SerializeField]
		private float mHorizontalScrollbarSpacing;
		public float horizontalScrollbarSpacing { get { return mHorizontalScrollbarSpacing; } set { mHorizontalScrollbarSpacing = value; SetDirty(); } }

		[SerializeField]
		private float mVerticalScrollbarSpacing;
		public float verticalScrollbarSpacing { get { return mVerticalScrollbarSpacing; } set { mVerticalScrollbarSpacing = value; SetDirty(); } }

		[SerializeField]
		private ScrollRectEvent mOnValueChanged = new ScrollRectEvent();
		public ScrollRectEvent onValueChanged { get { return mOnValueChanged; } set { mOnValueChanged = value; } }

		// The offset from handle position to mouse down position
		private Vector2 mPointerStartLocalCursor = Vector2.zero;
		private Vector2 mContentStartPosition = Vector2.zero;

		private RectTransform mViewRect;

		protected RectTransform viewRect
		{
			get
			{
				if (mViewRect == null)
					mViewRect = mViewport;
				if (mViewRect == null)
					mViewRect = (RectTransform)transform;
				return mViewRect;
			}
		}

		private Bounds mContentBounds;
		private Bounds mViewBounds;

		private Vector2 mVelocity;
		public Vector2 velocity { get { return mVelocity; } set { mVelocity = value; } }

		private bool mDragging;

		private Vector2 mPrevPosition = Vector2.zero;
		private Bounds mPrevContentBounds;
		private Bounds mPrevViewBounds;
		[NonSerialized]
		private bool mHasRebuiltLayout = false;

		private bool mHSliderExpand;
		private bool mVSliderExpand;
		private float mHSliderHeight;
		private float mVSliderWidth;

		[System.NonSerialized]
		private RectTransform mRect;
		private RectTransform rectTransform
		{
			get
			{
				if (mRect == null)
					mRect = GetComponent<RectTransform>();
				return mRect;
			}
		}

		private RectTransform mHorizontalScrollbarRect;
		private RectTransform mVerticalScrollbarRect;

		private DrivenRectTransformTracker mTracker;

		protected LoopScrollRect()
		{
			flexibleWidth = -1;
		}

		//==========LoopScrollRect==========

		public void ClearCells()
		{
			if (Application.isPlaying)
			{
				itemTypeStart = 0;
				itemTypeEnd = 0;
				totalCount = 0;
				objectsToFill = null;
				for (int i = content.childCount - 1; i >= 0; i--)
				{
					prefabSource.ReturnObject(content.GetChild(i));
				}
			}
		}

		public void SrollToCell(int index, float speed)
		{
			if (totalCount >= 0 && (index < 0 || index >= totalCount))
			{
				DebugUtility.LogWarning(LoggerTags.UI, "invalid index {0}", index);
				return;
			}
			if (speed <= 0)
			{
				DebugUtility.LogWarning(LoggerTags.UI, "invalid speed {0}", speed);
				return;
			}
			StopAllCoroutines();
			StartCoroutine(ScrollToCellCoroutine(index, speed));
		}

		IEnumerator ScrollToCellCoroutine(int index, float speed)
		{
			bool needMoving = true;
			while (needMoving)
			{
				yield return null;
				if (!mDragging)
				{
					float move = 0;
					if (index < itemTypeStart)
					{
						move = -Time.deltaTime * speed;
					}
					else if (index >= itemTypeEnd)
					{
						move = Time.deltaTime * speed;
					}
					else
					{
						mViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
						var mItemBounds = GetBounds4Item(index);
						var offset = 0.0f;
						if (directionSign == -1)
							offset = reverseDirection ? (mViewBounds.min.y - mItemBounds.min.y) : (mViewBounds.max.y - mItemBounds.max.y);
						else if (directionSign == 1)
							offset = reverseDirection ? (mItemBounds.max.x - mViewBounds.max.x) : (mItemBounds.min.x - mViewBounds.min.x);
						// check if we cannot move on
						if (totalCount >= 0)
						{
							if (offset > 0 && itemTypeEnd == totalCount && !reverseDirection)
							{
								mItemBounds = GetBounds4Item(totalCount - 1);
								// reach bottom
								if ((directionSign == -1 && mItemBounds.min.y > mViewBounds.min.y) ||
									(directionSign == 1 && mItemBounds.max.x < mViewBounds.max.x))
								{
									needMoving = false;
									break;
								}
							}
							else if (offset < 0 && itemTypeStart == 0 && reverseDirection)
							{
								mItemBounds = GetBounds4Item(0);
								if ((directionSign == -1 && mItemBounds.max.y < mViewBounds.max.y) ||
									(directionSign == 1 && mItemBounds.min.x > mViewBounds.min.x))
								{
									needMoving = false;
									break;
								}
							}
						}

						float maxMove = Time.deltaTime * speed;
						if (Mathf.Abs(offset) < maxMove)
						{
							needMoving = false;
							move = offset;
						}
						else
						{
							move = Mathf.Sign(offset) * maxMove;
						}
					}
					if (!move.NearlyZero())
					{
						Vector2 offset = GetVector(move);
						content.anchoredPosition += offset;
						mPrevPosition += offset;
						mContentStartPosition += offset;
					}
				}
			}
			StopMovement();
			UpdatePrevData();
		}

		public void RefreshCells()
		{
			if (Application.isPlaying && this.isActiveAndEnabled)
			{
				itemTypeEnd = itemTypeStart;
				// recycle items if we can
				for (int i = 0; i < content.childCount; i++)
				{
					if (itemTypeEnd < totalCount)
					{
						dataSource.ProvideData(content.GetChild(i), itemTypeEnd);
						itemTypeEnd++;
					}
					else
					{
						prefabSource.ReturnObject(content.GetChild(i));
						i--;
					}
				}
			}
		}

		public void RefillCellsFromEnd(int offset = 0)
		{
			if (!Application.isPlaying || prefabSource == null)
				return;

			StopMovement();
			itemTypeEnd = reverseDirection ? offset : totalCount - offset;
			itemTypeStart = itemTypeEnd;

			if (totalCount >= 0 && itemTypeStart % contentConstraintCount != 0)
				DebugUtility.LogWarning(LoggerTags.UI, "Grid will become strange since we can't fill items in the last line");

			for (int i = mContent.childCount - 1; i >= 0; i--)
			{
				prefabSource.ReturnObject(mContent.GetChild(i));
			}

			float sizeToFill = 0, sizeFilled = 0;
			if (directionSign == -1)
				sizeToFill = viewRect.rect.size.y;
			else
				sizeToFill = viewRect.rect.size.x;

			while (sizeToFill > sizeFilled)
			{
				float size = reverseDirection ? NewItemAtEnd() : NewItemAtStart();
				if (size <= 0) break;
				sizeFilled += size;
			}

			Vector2 pos = mContent.anchoredPosition;
			float dist = Mathf.Max(0, sizeFilled - sizeToFill);
			if (reverseDirection)
				dist = -dist;
			if (directionSign == -1)
				pos.y = dist;
			else if (directionSign == 1)
				pos.x = -dist;
			mContent.anchoredPosition = pos;
		}

		public void RefillCells(int offset = 0)
		{
			if (!Application.isPlaying || prefabSource == null)
				return;

			StopMovement();
			itemTypeStart = reverseDirection ? totalCount - offset : offset;
			itemTypeEnd = itemTypeStart;

			if (totalCount >= 0 && itemTypeStart % contentConstraintCount != 0)
				DebugUtility.LogWarning(LoggerTags.UI, "Grid will become strange since we can't fill items in the first line");

			// Don't `Canvas.ForceUpdateCanvases();` here, or it will new/delete cells to change itemTypeStart/End
			for (int i = mContent.childCount - 1; i >= 0; i--)
			{
				prefabSource.ReturnObject(mContent.GetChild(i));
			}

			float sizeToFill = 0, sizeFilled = 0;
			// mViewBounds may be not ready when RefillCells on Start
			if (directionSign == -1)
				sizeToFill = viewRect.rect.size.y;
			else
				sizeToFill = viewRect.rect.size.x;

			while (sizeToFill > sizeFilled)
			{
				float size = reverseDirection ? NewItemAtStart() : NewItemAtEnd();
				if (size <= 0) break;
				sizeFilled += size;
			}

			Vector2 pos = mContent.anchoredPosition;
			if (directionSign == -1)
				pos.y = 0;
			else if (directionSign == 1)
				pos.x = 0;
			mContent.anchoredPosition = pos;
		}

		protected float NewItemAtStart()
		{
			if (totalCount >= 0 && itemTypeStart - contentConstraintCount < 0)
			{
				return 0;
			}
			float size = 0;
			for (int i = 0; i < contentConstraintCount; i++)
			{
				itemTypeStart--;
				RectTransform newItem = InstantiateNextItem(itemTypeStart);
				newItem.SetAsFirstSibling();
				size = Mathf.Max(GetSize(newItem), size);
			}
			threshold = Mathf.Max(threshold, size * 1.5f);

			if (!reverseDirection)
			{
				Vector2 offset = GetVector(size);
				content.anchoredPosition += offset;
				mPrevPosition += offset;
				mContentStartPosition += offset;
			}

			return size;
		}

		protected float DeleteItemAtStart()
		{
			// special case: when moving or dragging, we cannot simply delete start when we've reached the end
			if (((mDragging || mVelocity != Vector2.zero) && totalCount >= 0 && itemTypeEnd >= totalCount - 1)
				|| content.childCount == 0)
			{
				return 0;
			}

			float size = 0;
			for (int i = 0; i < contentConstraintCount; i++)
			{
				RectTransform oldItem = content.GetChild(0) as RectTransform;
				size = Mathf.Max(GetSize(oldItem), size);
				prefabSource.ReturnObject(oldItem);

				itemTypeStart++;

				if (content.childCount == 0)
				{
					break;
				}
			}

			if (!reverseDirection)
			{
				Vector2 offset = GetVector(size);
				content.anchoredPosition -= offset;
				mPrevPosition -= offset;
				mContentStartPosition -= offset;
			}
			return size;
		}


		protected float NewItemAtEnd()
		{
			if (totalCount >= 0 && itemTypeEnd >= totalCount)
			{
				return 0;
			}
			float size = 0;
			// issue 4: fill lines to end first
			int count = contentConstraintCount - (content.childCount % contentConstraintCount);
			for (int i = 0; i < count; i++)
			{
				RectTransform newItem = InstantiateNextItem(itemTypeEnd);
				size = Mathf.Max(GetSize(newItem), size);
				itemTypeEnd++;
				if (totalCount >= 0 && itemTypeEnd >= totalCount)
				{
					break;
				}
			}
			threshold = Mathf.Max(threshold, size * 1.5f);

			if (reverseDirection)
			{
				Vector2 offset = GetVector(size);
				content.anchoredPosition -= offset;
				mPrevPosition -= offset;
				mContentStartPosition -= offset;
			}

			return size;
		}

		protected float DeleteItemAtEnd()
		{
			if (((mDragging || mVelocity != Vector2.zero) && totalCount >= 0 && itemTypeStart < contentConstraintCount)
				|| content.childCount == 0)
			{
				return 0;
			}

			float size = 0;
			for (int i = 0; i < contentConstraintCount; i++)
			{
				RectTransform oldItem = content.GetChild(content.childCount - 1) as RectTransform;
				size = Mathf.Max(GetSize(oldItem), size);
				prefabSource.ReturnObject(oldItem);

				itemTypeEnd--;
				if (itemTypeEnd % contentConstraintCount == 0 || content.childCount == 0)
				{
					break;  //just delete the whole row
				}
			}

			if (reverseDirection)
			{
				Vector2 offset = GetVector(size);
				content.anchoredPosition += offset;
				mPrevPosition += offset;
				mContentStartPosition += offset;
			}
			return size;
		}

		private RectTransform InstantiateNextItem(int itemIdx)
		{
			RectTransform nextItem = prefabSource.GetObject().transform as RectTransform;
			nextItem.transform.SetParent(content, false);
			nextItem.gameObject.SetActive(true);
			dataSource.ProvideData(nextItem, itemIdx);
			return nextItem;
		}
		//==========LoopScrollRect==========

		public virtual void Rebuild(CanvasUpdate executing)
		{
			if (executing == CanvasUpdate.Prelayout)
			{
				UpdateCachedData();
			}

			if (executing == CanvasUpdate.PostLayout)
			{
				UpdateBounds();
				UpdateScrollbars(Vector2.zero);
				UpdatePrevData();

				mHasRebuiltLayout = true;
			}
		}

		public virtual void LayoutComplete()
		{ }

		public virtual void GraphicUpdateComplete()
		{ }

		void UpdateCachedData()
		{
			Transform transform = this.transform;
			mHorizontalScrollbarRect = mHorizontalScrollbar == null ? null : mHorizontalScrollbar.transform as RectTransform;
			mVerticalScrollbarRect = mVerticalScrollbar == null ? null : mVerticalScrollbar.transform as RectTransform;

			// These are true if either the elements are children, or they don't exist at all.
			bool viewIsChild = (viewRect.parent == transform);
			bool hScrollbarIsChild = (!mHorizontalScrollbarRect || mHorizontalScrollbarRect.parent == transform);
			bool vScrollbarIsChild = (!mVerticalScrollbarRect || mVerticalScrollbarRect.parent == transform);
			bool allAreChildren = (viewIsChild && hScrollbarIsChild && vScrollbarIsChild);

			mHSliderExpand = allAreChildren && mHorizontalScrollbarRect && horizontalScrollbarVisibility == EScrollbarVisibility.AutoHideAndExpandViewport;
			mVSliderExpand = allAreChildren && mVerticalScrollbarRect && verticalScrollbarVisibility == EScrollbarVisibility.AutoHideAndExpandViewport;
			mHSliderHeight = (mHorizontalScrollbarRect == null ? 0 : mHorizontalScrollbarRect.rect.height);
			mVSliderWidth = (mVerticalScrollbarRect == null ? 0 : mVerticalScrollbarRect.rect.width);
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			if (mHorizontalScrollbar)
				mHorizontalScrollbar.onValueChanged.AddListener(SetHorizontalNormalizedPosition);
			if (mVerticalScrollbar)
				mVerticalScrollbar.onValueChanged.AddListener(SetVerticalNormalizedPosition);

			CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
		}

		protected override void OnDisable()
		{
			CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);

			if (mHorizontalScrollbar)
				mHorizontalScrollbar.onValueChanged.RemoveListener(SetHorizontalNormalizedPosition);
			if (mVerticalScrollbar)
				mVerticalScrollbar.onValueChanged.RemoveListener(SetVerticalNormalizedPosition);

			mHasRebuiltLayout = false;
			mTracker.Clear();
			mVelocity = Vector2.zero;
			LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
			base.OnDisable();
		}

		public override bool IsActive()
		{
			return base.IsActive() && mContent != null;
		}

		private void EnsureLayoutHasRebuilt()
		{
			if (!mHasRebuiltLayout && !CanvasUpdateRegistry.IsRebuildingLayout())
				Canvas.ForceUpdateCanvases();
		}

		public virtual void StopMovement()
		{
			mVelocity = Vector2.zero;
		}

		public virtual void OnScroll(PointerEventData data)
		{
			if (!IsActive())
				return;

			EnsureLayoutHasRebuilt();
			UpdateBounds();

			Vector2 delta = data.scrollDelta;
			// Down is positive for scroll events, while in UI system up is positive.
			delta.y *= -1;
			if (vertical && !horizontal)
			{
				if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
					delta.y = delta.x;
				delta.x = 0;
			}
			if (horizontal && !vertical)
			{
				if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
					delta.x = delta.y;
				delta.y = 0;
			}

			Vector2 position = mContent.anchoredPosition;
			position += delta * mScrollSensitivity;
			if (mMovementType == EMovementType.Clamped)
				position += CalculateOffset(position - mContent.anchoredPosition);

			SetContentAnchoredPosition(position);
			UpdateBounds();
		}

		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			mVelocity = Vector2.zero;
		}

		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			if (!IsActive())
				return;

			UpdateBounds();

			mPointerStartLocalCursor = Vector2.zero;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, eventData.position, eventData.pressEventCamera, out mPointerStartLocalCursor);
			mContentStartPosition = mContent.anchoredPosition;
			mDragging = true;
		}

		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			mDragging = false;
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
				return;

			if (!IsActive())
				return;

			Vector2 localCursor;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, eventData.position, eventData.pressEventCamera, out localCursor))
				return;

			UpdateBounds();

			var pointerDelta = localCursor - mPointerStartLocalCursor;
			Vector2 position = mContentStartPosition + pointerDelta;

			// Offset to get content into place in the view.
			Vector2 offset = CalculateOffset(position - mContent.anchoredPosition);
			position += offset;
			if (mMovementType == EMovementType.Elastic)
			{
				//==========LoopScrollRect==========
				if (offset.x != 0)
					position.x = position.x - RubberDelta(offset.x, mViewBounds.size.x) * rubberScale;
				if (offset.y != 0)
					position.y = position.y - RubberDelta(offset.y, mViewBounds.size.y) * rubberScale;
				//==========LoopScrollRect==========
			}

			SetContentAnchoredPosition(position);
		}

		protected virtual void SetContentAnchoredPosition(Vector2 position)
		{
			if (!mHorizontal)
				position.x = mContent.anchoredPosition.x;
			if (!mVertical)
				position.y = mContent.anchoredPosition.y;

			if (position != mContent.anchoredPosition)
			{
				mContent.anchoredPosition = position;
				UpdateBounds(true);
			}
		}

		protected virtual void LateUpdate()
		{
			if (!mContent)
				return;

			EnsureLayoutHasRebuilt();
			UpdateScrollbarVisibility();
			UpdateBounds();
			float deltaTime = Time.unscaledDeltaTime;
			Vector2 offset = CalculateOffset(Vector2.zero);
			if (!mDragging && (offset != Vector2.zero || mVelocity != Vector2.zero))
			{
				Vector2 position = mContent.anchoredPosition;
				for (int axis = 0; axis < 2; axis++)
				{
					// Apply spring physics if movement is elastic and content has an offset from the view.
					if (mMovementType == EMovementType.Elastic && offset[axis] != 0)
					{
						float speed = mVelocity[axis];
						position[axis] = Mathf.SmoothDamp(mContent.anchoredPosition[axis], mContent.anchoredPosition[axis] + offset[axis], ref speed, mElasticity, Mathf.Infinity, deltaTime);
						mVelocity[axis] = speed;
					}
					// Else move content according to velocity with deceleration applied.
					else if (mInertia)
					{
						mVelocity[axis] *= Mathf.Pow(mDecelerationRate, deltaTime);
						if (Mathf.Abs(mVelocity[axis]) < 1)
							mVelocity[axis] = 0;
						position[axis] += mVelocity[axis] * deltaTime;
					}
					// If we have neither elaticity or friction, there shouldn't be any velocity.
					else
					{
						mVelocity[axis] = 0;
					}
				}

				if (mVelocity != Vector2.zero)
				{
					if (mMovementType == EMovementType.Clamped)
					{
						offset = CalculateOffset(position - mContent.anchoredPosition);
						position += offset;
					}

					SetContentAnchoredPosition(position);
				}
			}

			if (mDragging && mInertia)
			{
				Vector3 newVelocity = (mContent.anchoredPosition - mPrevPosition) / deltaTime;
				mVelocity = Vector3.Lerp(mVelocity, newVelocity, deltaTime * 10);
			}

			if (mViewBounds != mPrevViewBounds || mContentBounds != mPrevContentBounds || mContent.anchoredPosition != mPrevPosition)
			{
				UpdateScrollbars(offset);
				mOnValueChanged.Invoke(normalizedPosition);
				UpdatePrevData();
			}
		}

		private void UpdatePrevData()
		{
			if (mContent == null)
				mPrevPosition = Vector2.zero;
			else
				mPrevPosition = mContent.anchoredPosition;
			mPrevViewBounds = mViewBounds;
			mPrevContentBounds = mContentBounds;
		}

		private void UpdateScrollbars(Vector2 offset)
		{
			if (mHorizontalScrollbar)
			{
				//==========LoopScrollRect==========
				if (mContentBounds.size.x > 0 && totalCount > 0)
				{
					mHorizontalScrollbar.size = Mathf.Clamp01((mViewBounds.size.x - Mathf.Abs(offset.x)) / mContentBounds.size.x * currentLines / totalLines);
				}
				//==========LoopScrollRect==========
				else
					mHorizontalScrollbar.size = 1;

				mHorizontalScrollbar.value = horizontalNormalizedPosition;
			}

			if (mVerticalScrollbar)
			{
				//==========LoopScrollRect==========
				if (mContentBounds.size.y > 0 && totalCount > 0)
				{
					mVerticalScrollbar.size = Mathf.Clamp01((mViewBounds.size.y - Mathf.Abs(offset.y)) / mContentBounds.size.y * currentLines / totalLines);
				}
				//==========LoopScrollRect==========
				else
					mVerticalScrollbar.size = 1;

				mVerticalScrollbar.value = verticalNormalizedPosition;
			}
		}

		public Vector2 normalizedPosition
		{
			get
			{
				return new Vector2(horizontalNormalizedPosition, verticalNormalizedPosition);
			}
			set
			{
				SetNormalizedPosition(value.x, 0);
				SetNormalizedPosition(value.y, 1);
			}
		}

		public float horizontalNormalizedPosition
		{
			get
			{
				UpdateBounds();
				//==========LoopScrollRect==========
				if (totalCount > 0 && itemTypeEnd > itemTypeStart)
				{
					//TODO: consider contentSpacing
					float elementSize = mContentBounds.size.x / currentLines;
					float totalSize = elementSize * totalLines;
					float offset = mContentBounds.min.x - elementSize * startLine;

					if (totalSize <= mViewBounds.size.x)
						return (mViewBounds.min.x > offset) ? 1 : 0;
					return (mViewBounds.min.x - offset) / (totalSize - mViewBounds.size.x);
				}
				else
					return 0.5f;
				//==========LoopScrollRect==========
			}
			set
			{
				SetNormalizedPosition(value, 0);
			}
		}

		public float verticalNormalizedPosition
		{
			get
			{
				UpdateBounds();
				//==========LoopScrollRect==========
				if (totalCount > 0 && itemTypeEnd > itemTypeStart)
				{
					//TODO: consider contentSpacinge
					float elementSize = mContentBounds.size.y / currentLines;
					float totalSize = elementSize * totalLines;
					float offset = mContentBounds.max.y + elementSize * startLine;

					if (totalSize <= mViewBounds.size.y)
						return (offset > mViewBounds.max.y) ? 1 : 0;
					return (offset - mViewBounds.max.y) / (totalSize - mViewBounds.size.y);
				}
				else
					return 0.5f;
				//==========LoopScrollRect==========
			}
			set
			{
				SetNormalizedPosition(value, 1);
			}
		}

		private void SetHorizontalNormalizedPosition(float value) { SetNormalizedPosition(value, 0); }
		private void SetVerticalNormalizedPosition(float value) { SetNormalizedPosition(value, 1); }

		private void SetNormalizedPosition(float value, int axis)
		{
			//==========LoopScrollRect==========
			if (totalCount <= 0 || itemTypeEnd <= itemTypeStart)
				return;
			//==========LoopScrollRect==========

			EnsureLayoutHasRebuilt();
			UpdateBounds();

			//==========LoopScrollRect==========
			Vector3 localPosition = mContent.localPosition;
			float newLocalPosition = localPosition[axis];
			if (axis == 0)
			{
				float elementSize = mContentBounds.size.x / currentLines;
				float totalSize = elementSize * totalLines;
				float offset = mContentBounds.min.x - elementSize * startLine;

				newLocalPosition += mViewBounds.min.x - value * (totalSize - mViewBounds.size[axis]) - offset;
			}
			else if (axis == 1)
			{
				float elementSize = mContentBounds.size.y / currentLines;
				float totalSize = elementSize * totalLines;
				float offset = mContentBounds.max.y + elementSize * startLine;

				newLocalPosition -= offset - value * (totalSize - mViewBounds.size.y) - mViewBounds.max.y;
			}
			//==========LoopScrollRect==========

			if (Mathf.Abs(localPosition[axis] - newLocalPosition) > 0.01f)
			{
				localPosition[axis] = newLocalPosition;
				mContent.localPosition = localPosition;
				mVelocity[axis] = 0;
				UpdateBounds(true);
			}
		}

		private static float RubberDelta(float overStretching, float viewSize)
		{
			return (1 - (1 / ((Mathf.Abs(overStretching) * 0.55f / viewSize) + 1))) * viewSize * Mathf.Sign(overStretching);
		}

		protected override void OnRectTransformDimensionsChange()
		{
			SetDirty();
		}

		private bool hScrollingNeeded
		{
			get
			{
				if (Application.isPlaying)
					return mContentBounds.size.x > mViewBounds.size.x + 0.01f;
				return true;
			}
		}
		private bool vScrollingNeeded
		{
			get
			{
				if (Application.isPlaying)
					return mContentBounds.size.y > mViewBounds.size.y + 0.01f;
				return true;
			}
		}

		public virtual void CalculateLayoutInputHorizontal() { }
		public virtual void CalculateLayoutInputVertical() { }

		public virtual float minWidth { get { return -1; } }
		public virtual float preferredWidth { get { return -1; } }
		public virtual float flexibleWidth { get; private set; }

		public virtual float minHeight { get { return -1; } }
		public virtual float preferredHeight { get { return -1; } }
		public virtual float flexibleHeight { get { return -1; } }

		public virtual int layoutPriority { get { return -1; } }

		public virtual void SetLayoutHorizontal()
		{
			mTracker.Clear();

			if (mHSliderExpand || mVSliderExpand)
			{
				mTracker.Add(this, viewRect,
					DrivenTransformProperties.Anchors |
					DrivenTransformProperties.SizeDelta |
					DrivenTransformProperties.AnchoredPosition);

				// Make view full size to see if content fits.
				viewRect.anchorMin = Vector2.zero;
				viewRect.anchorMax = Vector2.one;
				viewRect.sizeDelta = Vector2.zero;
				viewRect.anchoredPosition = Vector2.zero;

				// Recalculate content layout with this size to see if it fits when there are no scrollbars.
				LayoutRebuilder.ForceRebuildLayoutImmediate(content);
				mViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
				mContentBounds = GetBounds();
			}

			// If it doesn't fit vertically, enable vertical scrollbar and shrink view horizontally to make room for it.
			if (mVSliderExpand && vScrollingNeeded)
			{
				viewRect.sizeDelta = new Vector2(-(mVSliderWidth + mVerticalScrollbarSpacing), viewRect.sizeDelta.y);

				// Recalculate content layout with this size to see if it fits vertically
				// when there is a vertical scrollbar (which may reflowed the content to make it taller).
				LayoutRebuilder.ForceRebuildLayoutImmediate(content);
				mViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
				mContentBounds = GetBounds();
			}

			// If it doesn't fit horizontally, enable horizontal scrollbar and shrink view vertically to make room for it.
			if (mHSliderExpand && hScrollingNeeded)
			{
				viewRect.sizeDelta = new Vector2(viewRect.sizeDelta.x, -(mHSliderHeight + mHorizontalScrollbarSpacing));
				mViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
				mContentBounds = GetBounds();
			}

			// If the vertical slider didn't kick in the first time, and the horizontal one did,
			// we need to check again if the vertical slider now needs to kick in.
			// If it doesn't fit vertically, enable vertical scrollbar and shrink view horizontally to make room for it.
			if (mVSliderExpand && vScrollingNeeded && viewRect.sizeDelta.x == 0 && viewRect.sizeDelta.y < 0)
			{
				viewRect.sizeDelta = new Vector2(-(mVSliderWidth + mVerticalScrollbarSpacing), viewRect.sizeDelta.y);
			}
		}

		public virtual void SetLayoutVertical()
		{
			UpdateScrollbarLayout();
			mViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
			mContentBounds = GetBounds();
		}

		void UpdateScrollbarVisibility()
		{
			if (mVerticalScrollbar && mVerticalScrollbarVisibility != EScrollbarVisibility.Permanent && mVerticalScrollbar.gameObject.activeSelf != vScrollingNeeded)
				mVerticalScrollbar.gameObject.SetActive(vScrollingNeeded);

			if (mHorizontalScrollbar && mHorizontalScrollbarVisibility != EScrollbarVisibility.Permanent && mHorizontalScrollbar.gameObject.activeSelf != hScrollingNeeded)
				mHorizontalScrollbar.gameObject.SetActive(hScrollingNeeded);
		}

		void UpdateScrollbarLayout()
		{
			if (mVSliderExpand && mHorizontalScrollbar)
			{
				mTracker.Add(this, mHorizontalScrollbarRect,
							  DrivenTransformProperties.AnchorMinX |
							  DrivenTransformProperties.AnchorMaxX |
							  DrivenTransformProperties.SizeDeltaX |
							  DrivenTransformProperties.AnchoredPositionX);
				mHorizontalScrollbarRect.anchorMin = new Vector2(0, mHorizontalScrollbarRect.anchorMin.y);
				mHorizontalScrollbarRect.anchorMax = new Vector2(1, mHorizontalScrollbarRect.anchorMax.y);
				mHorizontalScrollbarRect.anchoredPosition = new Vector2(0, mHorizontalScrollbarRect.anchoredPosition.y);
				if (vScrollingNeeded)
					mHorizontalScrollbarRect.sizeDelta = new Vector2(-(mVSliderWidth + mVerticalScrollbarSpacing), mHorizontalScrollbarRect.sizeDelta.y);
				else
					mHorizontalScrollbarRect.sizeDelta = new Vector2(0, mHorizontalScrollbarRect.sizeDelta.y);
			}

			if (mHSliderExpand && mVerticalScrollbar)
			{
				mTracker.Add(this, mVerticalScrollbarRect,
							  DrivenTransformProperties.AnchorMinY |
							  DrivenTransformProperties.AnchorMaxY |
							  DrivenTransformProperties.SizeDeltaY |
							  DrivenTransformProperties.AnchoredPositionY);
				mVerticalScrollbarRect.anchorMin = new Vector2(mVerticalScrollbarRect.anchorMin.x, 0);
				mVerticalScrollbarRect.anchorMax = new Vector2(mVerticalScrollbarRect.anchorMax.x, 1);
				mVerticalScrollbarRect.anchoredPosition = new Vector2(mVerticalScrollbarRect.anchoredPosition.x, 0);
				if (hScrollingNeeded)
					mVerticalScrollbarRect.sizeDelta = new Vector2(mVerticalScrollbarRect.sizeDelta.x, -(mHSliderHeight + mHorizontalScrollbarSpacing));
				else
					mVerticalScrollbarRect.sizeDelta = new Vector2(mVerticalScrollbarRect.sizeDelta.x, 0);
			}
		}

		private void UpdateBounds(bool updateItems = false)
		{
			mViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
			mContentBounds = GetBounds();

			if (mContent == null)
				return;

			// ============LoopScrollRect============
			// Don't do this in Rebuild
			if (Application.isPlaying && updateItems && UpdateItems(mViewBounds, mContentBounds))
			{
				Canvas.ForceUpdateCanvases();
				mContentBounds = GetBounds();
			}
			// ============LoopScrollRect============

			// Make sure content bounds are at least as large as view by adding padding if not.
			// One might think at first that if the content is smaller than the view, scrolling should be allowed.
			// However, that's not how scroll views normally work.
			// Scrolling is *only* possible when content is *larger* than view.
			// We use the pivot of the content rect to decide in which directions the content bounds should be expanded.
			// E.g. if pivot is at top, bounds are expanded downwards.
			// This also works nicely when ContentSizeFitter is used on the content.
			Vector3 contentSize = mContentBounds.size;
			Vector3 contentPos = mContentBounds.center;
			Vector3 excess = mViewBounds.size - contentSize;
			if (excess.x > 0)
			{
				contentPos.x -= excess.x * (mContent.pivot.x - 0.5f);
				contentSize.x = mViewBounds.size.x;
			}
			if (excess.y > 0)
			{
				contentPos.y -= excess.y * (mContent.pivot.y - 0.5f);
				contentSize.y = mViewBounds.size.y;
			}

			mContentBounds.size = contentSize;
			mContentBounds.center = contentPos;
		}

		private readonly Vector3[] mCorners = new Vector3[4];
		private Bounds GetBounds()
		{
			if (mContent == null)
				return new Bounds();

			var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

			var toLocal = viewRect.worldToLocalMatrix;
			mContent.GetWorldCorners(mCorners);
			for (int j = 0; j < 4; j++)
			{
				Vector3 v = toLocal.MultiplyPoint3x4(mCorners[j]);
				vMin = Vector3.Min(v, vMin);
				vMax = Vector3.Max(v, vMax);
			}

			var bounds = new Bounds(vMin, Vector3.zero);
			bounds.Encapsulate(vMax);
			return bounds;
		}

		private Bounds GetBounds4Item(int index)
		{
			if (mContent == null)
				return new Bounds();

			var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

			var toLocal = viewRect.worldToLocalMatrix;
			int offset = index - itemTypeStart;
			if (offset < 0 || offset >= mContent.childCount)
				return new Bounds();
			var rt = mContent.GetChild(offset) as RectTransform;
			if (rt == null)
				return new Bounds();
			rt.GetWorldCorners(mCorners);
			for (int j = 0; j < 4; j++)
			{
				Vector3 v = toLocal.MultiplyPoint3x4(mCorners[j]);
				vMin = Vector3.Min(v, vMin);
				vMax = Vector3.Max(v, vMax);
			}

			var bounds = new Bounds(vMin, Vector3.zero);
			bounds.Encapsulate(vMax);
			return bounds;
		}

		private Vector2 CalculateOffset(Vector2 delta)
		{
			Vector2 offset = Vector2.zero;
			if (mMovementType == EMovementType.Unrestricted)
				return offset;

			Vector2 min = mContentBounds.min;
			Vector2 max = mContentBounds.max;

			if (mHorizontal)
			{
				min.x += delta.x;
				max.x += delta.x;
				if (min.x > mViewBounds.min.x)
					offset.x = mViewBounds.min.x - min.x;
				else if (max.x < mViewBounds.max.x)
					offset.x = mViewBounds.max.x - max.x;
			}

			if (mVertical)
			{
				min.y += delta.y;
				max.y += delta.y;
				if (max.y < mViewBounds.max.y)
					offset.y = mViewBounds.max.y - max.y;
				else if (min.y > mViewBounds.min.y)
					offset.y = mViewBounds.min.y - min.y;
			}

			return offset;
		}

		protected void SetDirty()
		{
			if (!IsActive())
				return;

			LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
		}

		protected void SetDirtyCaching()
		{
			if (!IsActive())
				return;

			CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
			LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
		}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			SetDirtyCaching();
		}
#endif
	}
}