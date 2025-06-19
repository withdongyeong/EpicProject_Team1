using System;
using System.Collections.Generic;

/// <summary>
/// 개별 패턴을 감싸는 래퍼 클래스
/// </summary>
public class PatternElement
{
    private IBossAttackPattern _pattern;
    private float _intervalAfterExecution;

    /// <summary>
    /// 패턴 프로퍼티
    /// </summary>
    public IBossAttackPattern Pattern { get => _pattern; }

    /// <summary>
    /// 패턴 실행 후 대기시간 프로퍼티
    /// </summary>
    public float IntervalAfterExecution { get => _intervalAfterExecution; }

    /// <summary>
    /// PatternElement 생성자
    /// </summary>
    /// <param name="pattern">실행할 패턴</param>
    /// <param name="intervalAfterExecution">패턴 실행 후 대기시간</param>
    public PatternElement(IBossAttackPattern pattern, float intervalAfterExecution)
    {
        _pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
        _intervalAfterExecution = Math.Max(0f, intervalAfterExecution);
    }
}