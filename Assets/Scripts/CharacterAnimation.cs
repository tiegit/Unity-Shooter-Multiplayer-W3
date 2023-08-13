using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private const string Grounded = "Grounded";
    private const string Speed = "Speed";

    private const string SitDown = "Sit";
    private const string StandUp = "Stand";

    [SerializeField] private Animator _footAnimator;
    [SerializeField] private CheckFly _checkFly;
    [SerializeField] private Character _character;
    [SerializeField] private Animator _playerAnimator;

    private void Start()
    {
        _character.sit += Sit;
        _character.stand += Stand;
    }

    private void Update()
    {
        Vector3 localVelocity = _character.transform.InverseTransformVector(_character.Velocity);
        float speed = localVelocity.magnitude / _character.Speed;
        float sign = Mathf.Sign(localVelocity.z);

        _footAnimator.SetFloat(Speed, speed * sign);
        _footAnimator.SetBool(Grounded, _checkFly.IsFly == false);
    }

    private void Sit()
    {
        _playerAnimator.SetBool(SitDown, true);
        _playerAnimator.SetBool(StandUp, false);
    }

    private void Stand()
    {
        _playerAnimator.SetBool(StandUp, true);
        _playerAnimator.SetBool(SitDown, false);
    }

    private void OnDestroy()
    {
        _character.sit -= Sit;
        _character.stand -= Stand;
    }


}
