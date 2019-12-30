public interface ICardEffect {
    int executionOrder { get; }
    string statusText { get; }

    void execEffect(PlayerController player, EnemyController enemy, CardTypes cardtype);
}