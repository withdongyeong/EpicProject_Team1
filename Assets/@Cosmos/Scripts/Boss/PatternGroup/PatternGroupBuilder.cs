using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 패턴 그룹을 빌드하는 빌더 클래스 (개선된 버전)
/// </summary>
public class PatternGroupBuilder
{
    private List<PatternElement> _patterns = new List<PatternElement>();
    private bool _isGroupIntervalSet = false;
    private float _groupInterval = 0f;
    private Action<ExecutableUnit> _onComplete;
    private bool _isDisposed = false;

    /// <summary>
    /// PatternGroupBuilder 생성자
    /// </summary>
    /// <param name="onComplete">빌드 완료 시 호출될 콜백</param>
    public PatternGroupBuilder(Action<ExecutableUnit> onComplete)
    {
        _onComplete = onComplete ?? throw new ArgumentNullException(nameof(onComplete));
        Debug.Log("PatternGroupBuilder created");
    }

    /// <summary>
    /// 그룹에 패턴 추가
    /// </summary>
    /// <param name="pattern">추가할 패턴</param>
    /// <param name="intervalAfterExecution">패턴 실행 후 대기시간</param>
    /// <returns>빌더 인스턴스 (체이닝용)</returns>
    public PatternGroupBuilder AddPattern(IBossAttackPattern pattern, float intervalAfterExecution)
    {
        if (pattern == null)
        {
            Debug.LogError("PatternGroupBuilder.AddPattern: pattern is null!");
            throw new ArgumentNullException(nameof(pattern));
        }

        if (_isGroupIntervalSet)
        {
            Debug.LogError("PatternGroupBuilder: Cannot add patterns after SetGroupInterval has been called!");
            throw new InvalidOperationException("Cannot add patterns after SetGroupInterval has been called");
        }

        _patterns.Add(new PatternElement(pattern, intervalAfterExecution));
        Debug.Log($"PatternGroupBuilder: Added pattern {pattern.PatternName}, total patterns: {_patterns.Count}");
        return this;
    }

    /// <summary>
    /// 그룹 완료 후 인터벌 설정 (필수 호출)
    /// </summary>
    /// <param name="intervalAfterGroup">그룹 완료 후 대기시간</param>
    public void SetGroupInterval(float intervalAfterGroup)
    {
        Debug.Log($"PatternGroupBuilder.SetGroupInterval called with interval: {intervalAfterGroup}");
        
        if (_isGroupIntervalSet)
        {
            Debug.LogError("PatternGroupBuilder: SetGroupInterval has already been called!");
            throw new InvalidOperationException("SetGroupInterval has already been called");
        }

        if (_patterns.Count == 0)
        {
            Debug.LogError("PatternGroupBuilder: Cannot set group interval without any patterns!");
            throw new InvalidOperationException("Cannot set group interval without any patterns");
        }

        _isGroupIntervalSet = true;
        _groupInterval = Math.Max(0f, intervalAfterGroup);

        // 그룹 빌드 완료
        try
        {
            BuildGroup();
            _isDisposed = true; // 정상 완료 표시
        }
        catch (Exception ex)
        {
            Debug.LogError($"PatternGroupBuilder.SetGroupInterval: BuildGroup failed - {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// 그룹을 실제로 빌드하고 완료 콜백 호출
    /// </summary>
    private void BuildGroup()
    {
        Debug.Log($"PatternGroupBuilder.BuildGroup: Building group with {_patterns.Count} patterns");
        
        try
        {
            PatternGroup group = new PatternGroup(_groupInterval);
            
            foreach (PatternElement patternElement in _patterns)
            {
                if (patternElement?.Pattern == null)
                {
                    Debug.LogError("PatternGroupBuilder.BuildGroup: Found null pattern element!");
                    throw new InvalidOperationException("Pattern element or its Pattern is null");
                }
                group.AddPattern(patternElement);
                Debug.Log($"PatternGroupBuilder.BuildGroup: Added pattern {patternElement.Pattern.PatternName} to group");
            }

            ExecutableUnit executableUnit = new ExecutableUnit(group);
            Debug.Log($"PatternGroupBuilder.BuildGroup: ExecutableUnit created successfully");
            
            _onComplete?.Invoke(executableUnit);
            Debug.Log($"PatternGroupBuilder: Group built successfully with {_patterns.Count} patterns and {_groupInterval}s group interval");
        }
        catch (Exception ex)
        {
            Debug.LogError($"PatternGroupBuilder.BuildGroup: Failed to build PatternGroup - {ex.Message}\nStackTrace: {ex.StackTrace}");
            throw;
        }
    }

    /// <summary>
    /// 빌더가 완료되었는지 확인
    /// </summary>
    public bool IsCompleted { get => _isGroupIntervalSet; }

    /// <summary>
    /// 현재 추가된 패턴 개수
    /// </summary>
    public int PatternCount { get => _patterns.Count; }

    /// <summary>
    /// 리소스 정리 (소멸자 대신 명시적 호출)
    /// </summary>
    public void Dispose()
    {
        if (!_isDisposed && !_isGroupIntervalSet && _patterns.Count > 0)
        {
            Debug.LogWarning($"PatternGroupBuilder disposed without calling SetGroupInterval! {_patterns.Count} patterns were added but not finalized.");
        }
        _isDisposed = true;
    }
}