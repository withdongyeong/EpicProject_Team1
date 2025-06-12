/// <summary>
/// 보스 공격 패턴 인터페이스 - 모든 보스 공격 패턴이 구현해야 하는 기본 인터페이스
/// </summary>
public interface IBossAttackPattern
{
    /// <summary>
    /// 패턴 이름 - 디버깅 및 식별용
    /// </summary>
    string PatternName { get; }
    
    /// <summary>
    /// 패턴 실행 - 실제 공격 로직 구현
    /// </summary>
    /// <param name="boss">공격을 실행하는 보스</param>
    void Execute(BaseBoss boss);
    
    /// <summary>
    /// 패턴 사용 가능 조건 체크 - 패턴 실행 전 조건 확인
    /// </summary>
    /// <param name="boss">조건을 체크할 보스</param>
    /// <returns>사용 가능 여부</returns>
    bool CanExecute(BaseBoss boss);
}