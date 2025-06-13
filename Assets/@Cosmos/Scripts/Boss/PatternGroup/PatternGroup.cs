using System;
using System.Collections.Generic;

/// <summary>
/// 패턴 그룹 클래스 - 순차적으로 실행되는 패턴들의 묶음
/// </summary>
public class PatternGroup
{
    private List<PatternElement> _patterns = new List<PatternElement>();
    private float _intervalAfterGroup;

    /// <summary>
    /// 그룹 내 패턴들 프로퍼티 (읽기 전용)
    /// </summary>
    public IReadOnlyList<PatternElement> Patterns { get => _patterns.AsReadOnly(); }

    /// <summary>
    /// 그룹 완료 후 대기시간 프로퍼티
    /// </summary>
    public float IntervalAfterGroup { get => _intervalAfterGroup; }

    /// <summary>
    /// PatternGroup 생성자
    /// </summary>
    /// <param name="intervalAfterGroup">그룹 완료 후 대기시간</param>
    public PatternGroup(float intervalAfterGroup)
    {
        _intervalAfterGroup = Math.Max(0f, intervalAfterGroup);
    }

    /// <summary>
    /// 그룹에 패턴 추가
    /// </summary>
    /// <param name="patternElement">추가할 패턴 요소</param>
    public void AddPattern(PatternElement patternElement)
    {
        if (patternElement == null)
            throw new ArgumentNullException(nameof(patternElement));
            
        _patterns.Add(patternElement);
    }

    /// <summary>
    /// 그룹에 패턴과 인터벌을 직접 추가
    /// </summary>
    /// <param name="pattern">추가할 패턴</param>
    /// <param name="intervalAfterExecution">패턴 실행 후 대기시간</param>
    public void AddPattern(IBossAttackPattern pattern, float intervalAfterExecution)
    {
        AddPattern(new PatternElement(pattern, intervalAfterExecution));
    }

    /// <summary>
    /// 그룹이 비어있는지 확인
    /// </summary>
    public bool IsEmpty { get => _patterns.Count == 0; }
}