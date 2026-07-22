using UnityEngine;

[System.Serializable]
public class KuponInfo
{
    public string nomorRegistrasi;
    public string namaPengungsi;
    public string tanggalTerbit;
    public Sprite stempelSprite;

    public bool asli; // TRUE = sah, FALSE = palsu

    public enum JenisKecacatan { TidakAda, NomorFormatSalah, TanggalKadaluarsa, StempelPalsu }
    public JenisKecacatan kecacatan = JenisKecacatan.TidakAda;
}