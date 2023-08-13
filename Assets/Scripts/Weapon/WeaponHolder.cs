using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] private byte _selectedWeapon = 0;

    private float _mouseScroll;
    private byte _totalWeapons;

    void Start()
    {
        _totalWeapons = (byte)transform.childCount;
        SelectWeapon(); // ��������� 0 ������
    }

    public void SetScrollChange(float mouseScroll)
    {
        _mouseScroll = mouseScroll;

        ChangeWeapon();
    }

    private void SelectWeapon() // ��������� ���������� ������ �� ������� � �������� ���������
    {
        int _weaponIndex = 0;
        foreach (Transform weapon in transform) // ����� �������� ���������� ������ � �������� ���������
        {
            if (_weaponIndex == _selectedWeapon)
            {
                weapon.gameObject.SetActive(true); // ��� ���������
            }
            else
            {
                weapon.gameObject.SetActive(false); // ����������� ������
            }
            _weaponIndex++;
        }
    }

    private void ChangeWeapon() // �������� ������� ���� � ����������� ����������� ������ ������
    {
        int prviousSelectedWeapon = _selectedWeapon;

        if (_mouseScroll > 0)
        {
            if (_selectedWeapon >= _totalWeapons - 1) _selectedWeapon = 0;
            else _selectedWeapon++;
        }

        if (_mouseScroll < 0)
        {
            if (_selectedWeapon <= 0) _selectedWeapon = (byte)(_totalWeapons - 1);
            else _selectedWeapon--;
        }

        if (prviousSelectedWeapon != _selectedWeapon) SelectWeapon();
    }

    public byte GetSelectWeaponIndex()
    {
        return _selectedWeapon;        
    }

    public void SetWeaponIndex(byte weaponIndex)
    {
        _selectedWeapon = weaponIndex;
        SelectWeapon();
    }
}
