using System.Collections;

/// <summary>
/// 보스 공격 패턴 인터페이스 - 코루틴 기반으로 수정
/// </summary>
public interface IBossAttackPattern
{
    /// <summary>
    /// 패턴 이름
    /// </summary>
    string PatternName { get; }

    /// <summary>
    /// 패턴 실행 가능 여부 확인
    /// </summary>
    /// <param name="boss">보스 객체</param>
    /// <returns>실행 가능 여부</returns>
    bool CanExecute(BaseBoss boss);

    /// <summary>
    /// 패턴 실행 (코루틴으로 변경)
    /// </summary>
    /// <param name="boss">보스 객체</param>
    /// <returns>패턴 실행 코루틴</returns>
    IEnumerator Execute(BaseBoss boss);
}