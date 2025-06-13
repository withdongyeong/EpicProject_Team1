using System;

/// <summary>
/// 실행 가능한 단위 - 개별 패턴 또는 패턴 그룹
/// </summary>
public class ExecutableUnit
{
    private PatternElement _individualPattern;
    private PatternGroup _patternGroup;
    private bool _isGroup;

    /// <summary>
    /// 개별 패턴인지 확인
    /// </summary>
    public bool IsIndividualPattern { get => !_isGroup; }

    /// <summary>
    /// 그룹 패턴인지 확인
    /// </summary>
    public bool IsGroup { get => _isGroup; }

    /// <summary>
    /// 개별 패턴 프로퍼티 (개별 패턴인 경우에만 유효)
    /// </summary>
    public PatternElement IndividualPattern 
    { 
        get 
        {
            if (_isGroup)
                throw new InvalidOperationException("This ExecutableUnit is a group, not an individual pattern");
            return _individualPattern;
        } 
    }

    /// <summary>
    /// 패턴 그룹 프로퍼티 (그룹인 경우에만 유효)
    /// </summary>
    public PatternGroup PatternGroup 
    { 
        get 
        {
            if (!_isGroup)
                throw new InvalidOperationException("This ExecutableUnit is an individual pattern, not a group");
            return _patternGroup;
        } 
    }

    /// <summary>
    /// 개별 패턴으로 ExecutableUnit 생성
    /// </summary>
    /// <param name="individualPattern">개별 패턴</param>
    public ExecutableUnit(PatternElement individualPattern)
    {
        _individualPattern = individualPattern ?? throw new ArgumentNullException(nameof(individualPattern));
        _isGroup = false;
    }

    /// <summary>
    /// 패턴 그룹으로 ExecutableUnit 생성
    /// </summary>
    /// <param name="patternGroup">패턴 그룹</param>
    public ExecutableUnit(PatternGroup patternGroup)
    {
        _patternGroup = patternGroup ?? throw new ArgumentNullException(nameof(patternGroup));
        if (_patternGroup.IsEmpty)
            throw new ArgumentException("PatternGroup cannot be empty", nameof(patternGroup));
        _isGroup = true;
    }
}