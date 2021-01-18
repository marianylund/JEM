using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    private Dictionary<string, bool> _thingsToLearn = new Dictionary<string, bool>();
    private Transform _hand;
    private Animator _handAnim;
    private Coroutine currentRoutine = null;
    private Queue<string> coroutineQueue = new Queue<string>();
    void Start()
    {
        _hand = transform.GetChild(0);
        Debug.Assert(_hand != null, "No hand in children found");
        _handAnim = _hand.GetComponentInChildren<Animator>();

        Debug.Assert(_handAnim != null, "No animator in children found");
        HideHand();

        _thingsToLearn.Add("plantedSeed", false);
        _thingsToLearn.Add("draggedAwayCloud", false);
        _thingsToLearn.Add("killedEnemy", false);
        _thingsToLearn.Add("draggedRain", false);

        if(_thingsToLearn.Values.All(V => V == true))
        {
            Debug.Log("Everything has been learned.");
        }

        LevelProgression.onMilestoneReached += MilestoneReached;
        currentRoutine = StartCoroutine(FindAndPlantASeed());
        SunBehaviour.onCloudedChange += TeachAboutTheCloud;
        ScenesLoader.onChangingScene += OnChangingScene;

    }

    private void TeachAboutTheCloud(bool isClouded)
    {
        if (!isClouded)
            return;
        if (currentRoutine != null)
            coroutineQueue.Enqueue("FindAndDragAwayCloud");
        else
            currentRoutine = StartCoroutine(FindAndDragAwayCloud());
        SunBehaviour.onCloudedChange -= TeachAboutTheCloud;
    }

    private void MilestoneReached(float milestone)
    {
        if (milestone == Settings.s.pastIntroduceEnemies)
        {
            if (currentRoutine != null)
                coroutineQueue.Enqueue("FindAndDestroyEnemy");
            else
                currentRoutine = StartCoroutine(FindAndDestroyEnemy());
        }
    }

    private void OnChangingScene(Level lvl)
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _Destroy();
    }

    private void _Destroy()
    {
        SunBehaviour.onCloudedChange -= TeachAboutTheCloud;
        LevelProgression.onMilestoneReached -= MilestoneReached;
        ScenesLoader.onChangingScene -= OnChangingScene;

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
    }

    private IEnumerator FindAndPlantASeed()
    {
        FallingSeedBehaviour fallingSeed = null;
        while (fallingSeed == null)
        {
            yield return new WaitForSeconds(1.0f);
            fallingSeed = GameObject.FindObjectsOfType<FallingSeedBehaviour>()?.FirstOrDefault(x => x.isActiveAndEnabled && MainCameraBehaviour.IsWithinView(x.transform.position));
            Debug.Log("Searching for seed");
        }
        Debug.Log("Found seed! " + fallingSeed.name);
        _hand.transform.position = Camera.main.transform.position.With(z: _hand.transform.position.z);
        _hand.gameObject.SetActive(true);
        SetHandToSimple();

        float speed = (Settings.s.beeFlyingSpeed + Settings.s.seedFallingSpeed) * 2.0f;
        while (!AreCloseEnough(fallingSeed.transform))
        {
            MoveTo(fallingSeed.transform, speed);
            yield return new WaitForEndOfFrame();
            if (!fallingSeed.isActiveAndEnabled)
            {
                _thingsToLearn["plantedSeed"] = true;
                break;
            }
        }

        if (!_thingsToLearn["plantedSeed"])
        {
            StartHover();
            StartDragging();
            fallingSeed.OnTutorialStart(_hand);

            speed = 1.8f;
            while (fallingSeed.isDragging)
            {
                DragItTo(fallingSeed.transform, speed, (Vector3.down + Vector3.right) * 100.0f);
                yield return new WaitForEndOfFrame();
            }

            fallingSeed.OnTutorialStop();
        }

        EndCoroutine();
    }

    private void DragItTo(Transform obj, float speed, Vector3 direction)
    {
        float step = speed * Time.deltaTime; // calculate distance to move
        Vector3 new_pos = Vector3.MoveTowards(_hand.position, _hand.position + direction, step);
        _hand.position = new_pos;
        obj.position = new_pos;
    }

    private void MoveTo(Transform obj, float speed)
    {
        float step = speed * Time.deltaTime; // calculate distance to move
        _hand.position = Vector3.MoveTowards(_hand.position, obj.position, step); 
    }

    private IEnumerator FindAndDragAwayCloud()
    {
        CloudBehaviour cloud = null;
        while (cloud == null)
        {
            yield return new WaitForSeconds(1.0f);
            cloud = GameObject.FindObjectsOfType<CloudBehaviour>()?.FirstOrDefault(x => x.isActiveAndEnabled && MainCameraBehaviour.IsWithinView(x.transform.position));
            Debug.Log("Searching for cloud");
        }
        Debug.Log("Found cloud! " + cloud.name);
        _hand.transform.position = Camera.main.transform.position.With(z: _hand.transform.position.z);
        _hand.gameObject.SetActive(true);
        SetHandToSimple();

        float speed = 2.0f;
        while (!AreCloseEnough(cloud.transform))
        {
            MoveTo(cloud.transform, speed);
            yield return new WaitForEndOfFrame();
            if (!MainCameraBehaviour.IsWithinView(cloud.transform.position))
            {
                _thingsToLearn["draggedAwayCloud"] = true;
                break;
            }
        }

        if (!_thingsToLearn["draggedAwayCloud"])
        {
            SunBehaviour.onCloudedChange += (bool isClouded) => { if (isClouded == false) { _thingsToLearn["draggedAwayCloud"] = true; } };

            StartHover();
            StartDragging();
            cloud.OnTutorialStart(_hand);

            speed = Settings.s.beeFlyingSpeed * 2.0f;
            while (MainCameraBehaviour.IsWithinView(cloud.transform.position))
            {
                DragItTo(cloud.transform, speed, (Vector3.left) * 100.0f);
                yield return new WaitForEndOfFrame();
            }


            cloud.OnTutorialStop();
            SunBehaviour.onCloudedChange -= (bool isClouded) => { if (isClouded == false) { _thingsToLearn["draggedAwayCloud"] = true; } };
            _thingsToLearn["draggedAwayCloud"] = true;
        }

        EndCoroutine();
    }

    private IEnumerator FindAndDestroyEnemy()
    {
        EnemyBehaviour enemy = null;
        while (enemy == null)
        {
            yield return new WaitForSeconds(1.0f);
            enemy = GameObject.FindObjectsOfType<EnemyBehaviour>()?.FirstOrDefault(x => x.isActiveAndEnabled && MainCameraBehaviour.IsWithinView(x.transform.position));
            Debug.Log("Searching for Enemy");
        }
        Debug.Log("Found Enemy! " + enemy.name);
        _hand.transform.position = Camera.main.transform.position.With(z: _hand.transform.position.z);
        _hand.gameObject.SetActive(true);
        SetHandToSimple();

        float speed = (Settings.s.beeFlyingSpeed + Settings.s.enemyMovementSpeed) * 2.0f;
        while (!AreCloseEnough(enemy.transform))
        {
            MoveTo(enemy.transform, speed);
            yield return new WaitForEndOfFrame();
            if (!enemy.isActiveAndEnabled)
            {
                _thingsToLearn["killedEnemy"] = true;
                break;
            }
        }

        print("Got close enough!");
        if (!_thingsToLearn["killedEnemy"])
        {
            StartHover();
            _hand.parent = enemy.transform;
            float _timer = 0.0f;
            _handAnim.SetTrigger("OnTap");
            enemy.EnemyGetHit();

            int order = 1;
            EnemyBehaviour.onEnemyEliminated += (Vector3 position, EnemyType type) => { _thingsToLearn["killedEnemy"] = true; };
            float wait_time = 0.6f;

            while (_thingsToLearn["killedEnemy"] != true)
            {
                _timer += Time.deltaTime;
                if(_timer > wait_time)
                {
                    if (order == 1)
                    {
                        _handAnim.SetTrigger("OnDoubleTap");
                        wait_time = 1.8f;
                    }
                    else if (order == 2)
                        enemy.EnemyGetHit();
                    _timer = 0.0f;
                    order++;
                }
                yield return new WaitForEndOfFrame();
            }
        }

        EnemyBehaviour.onEnemyEliminated -= (Vector3 position, EnemyType type) => { _thingsToLearn["killedEnemy"] = true; };

        _hand.parent = transform;
        EndCoroutine();
    }

    private void EndCoroutine()
    {
        HideHand();
        currentRoutine = null;
        if(coroutineQueue.Count != 0)
        {
            currentRoutine = StartCoroutine(coroutineQueue.Dequeue());
        }
    }

    private bool AreCloseEnough(Transform obj)
    {
        Vector2 handPos = _hand.position;
        Vector2 objPos = obj.position;
        return Mathf.Abs(Vector2.Distance(handPos, objPos)) < 0.1f;
    }
    private void HideHand()
    {
        _hand.gameObject.SetActive(false);
        SetHandToSimple();
    }
    private void SetHandToSimple()
    {
        _handAnim.SetBool("IsDragging", false);
        _handAnim.SetBool("IsHovering", false);
    }

    private void StartHover()
    {
        _handAnim.SetBool("IsHovering", true);
        _handAnim.SetBool("IsDragging", false);
    }

    private void StartDragging()
    {
        _handAnim.SetBool("IsHovering", true);
        _handAnim.SetBool("IsDragging", true);
    }
    private void StopDragging()
    {
        _handAnim.SetBool("IsDragging", false);
    }



}
