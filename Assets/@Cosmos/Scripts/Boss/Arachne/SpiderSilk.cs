using System.Collections;
using UnityEngine;

public class SpiderSilk : MonoBehaviour
{
    private Vector3 _basePos;
    private float _targetWidth = 1f;
    private float _growTime = 1.4f;
    private bool _isGrowing = true;

    private Vector3 _startScale;
    private Vector3 _endScale;

    private Transform _playerTransform;

    public void Init(Vector3 basePos, float targetWidth)
    {
        _basePos = basePos;
        _targetWidth = targetWidth;

        _startScale = transform.localScale;
        _endScale = new Vector3(_targetWidth, 0.3f, 1);

        StartCoroutine(Grow());
    }

    private IEnumerator Grow()
    {
        SoundManager.Instance.ArachneSoundClip("SpiderSilkActivate");

        float elapsed = 0f;

        while (elapsed < _growTime && _isGrowing)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _growTime;

            float currentWidth = Mathf.Lerp(_startScale.x, _endScale.x, t);
            transform.localScale = new Vector3(currentWidth * 2f, 0.3f, 1);
            transform.position = _basePos - new Vector3((currentWidth - _startScale.x), 0, 0);

            yield return null;
        }

        // 자라다 멈췄다면 종료. 아니면 사라짐
        if (_isGrowing)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponent<PlayerController>();

        if (collision.GetComponent<PlayerController>() != null)
        {
            _isGrowing = false;
            _playerTransform = collision.transform;
            playerController.Bind(0.3f);

            StartCoroutine(ShrinkAndPullPlayer(playerController));
        }
    }

    private IEnumerator ShrinkAndPullPlayer(PlayerController playerController)
    {
        float shrinkTime = 0.25f;
        float elapsed = 0f;

        Vector3 currentScale = transform.localScale;
        Vector3 startPos = transform.position;
        Vector3 endScale = new Vector3(0.1f, 0.3f, 1);

        Vector3 playerStartPos = _playerTransform.position;
        Vector3 playerEndPos = GridManager.Instance.GridToWorldPosition(new Vector3Int(7, playerController.CurrentY, 0));

        while (elapsed < shrinkTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / shrinkTime;

            float newWidth = Mathf.Lerp(currentScale.x, endScale.x, t);
            transform.localScale = new Vector3(newWidth, 0.3f, 1);
            transform.position = _basePos - new Vector3((newWidth - 0.1f) / 2f, 0, 0);

            _playerTransform.position = Vector3.Lerp(playerStartPos, playerEndPos, t);

            yield return null;
        }
        playerController.CurrentX = 8;
        playerController.CurrentY = (int)this.transform.position.y + 4;

        _playerTransform.position = GridManager.Instance.GridToWorldPosition(new Vector3Int(playerController.CurrentX, playerController.CurrentY, 0));

        Destroy(gameObject);
    }
}
