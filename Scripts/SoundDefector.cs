using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDefector : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip jumpSound, coinSound, winSound, loseSound, damageSound;

    public void PlayJumpSound()
    {
        audioSource.PlayOneShot(jumpSound);
    }
    public void PlayCoinSound()
    {
        audioSource.PlayOneShot(coinSound);
    }
    public void PlayWinSound()
    {
        audioSource.PlayOneShot(winSound);
    }
    public void PlayLoseSound()
    {
        audioSource.PlayOneShot(loseSound);
    }
    public void PlayDamageSound()
    {
        audioSource.PlayOneShot(damageSound);
    }
}
