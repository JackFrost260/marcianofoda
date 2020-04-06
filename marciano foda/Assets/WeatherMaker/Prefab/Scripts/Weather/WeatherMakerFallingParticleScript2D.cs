﻿//
// Weather Maker for Unity
// (c) 2016 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 
// *** A NOTE ABOUT PIRACY ***
// 
// If you got this asset from a pirate site, please consider buying it from the Unity asset store at https://www.assetstore.unity3d.com/en/#!/content/60955?aid=1011lGnL. This asset is only legally available from the Unity Asset Store.
// 
// I'm a single indie dev supporting my family by spending hundreds and thousands of hours on this and other assets. It's very offensive, rude and just plain evil to steal when I (and many others) put so much hard work into the software.
// 
// Thank you.
//
// *** END NOTE ABOUT PIRACY ***
//

using UnityEngine;

using System;

namespace DigitalRuby.WeatherMaker
{
    public class WeatherMakerFallingParticleScript2D : WeatherMakerFallingParticleScript
    {
        [Header("2D Settings")]
        [Tooltip("The starting y offset for particles and mist. This will be offset as a percentage of visible height from the top of the visible world.")]
        public float HeightMultiplier = 0.15f;

        [Tooltip("The total width of the particles and mist as a percentage of visible width")]
        public float WidthMultiplier = 1.5f;

        [Tooltip("Collision mask for the particles")]
        public LayerMask CollisionMask = -1;

        [Tooltip("Lifetime to assign to particles that have collided. 0 for instant death. This can allow the particles to penetrate a little bit beyond the collision point.")]
        [Range(0.0f, 5.0f)]
        public float CollisionLifeTime = 0.02f;

        [Tooltip("When a particles life time is less than or equal to this value, it may emit an explosion")]
        [Range(0.0f, 1.0f)]
        public float ExplosionEmissionLifeTimeMaximum = 1.0f / 60.0f;

        [Tooltip("Multiply the velocity of any mist colliding by this amount")]
        [Range(0.0f, 1.0f)]
        public float MistCollisionVelocityMultiplier = 0.8f;

        [Tooltip("Multiply the life time of any mist colliding by this amount")]
        [Range(0.0f, 1.0f)]
        public float MistCollisionLifeTimeMultiplier = 0.95f;

        private float cameraMultiplier;
        private float yOffset;
        private float visibleWorldWidth;

        private static readonly ParticleSystem.Particle[] particles = new ParticleSystem.Particle[8192];

        private void TransformParticleSystem(ParticleSystem particles, Camera camera, Vector2 initialStartSpeed, System.Collections.Generic.KeyValuePair<Vector3, Vector3> initialStartSize)
        {
            if (particles == null)
            {
                return;
            }
            Transform anchor = camera.transform;
            particles.transform.position = new Vector3(anchor.position.x, anchor.position.y + camera.orthographicSize + yOffset, particles.transform.position.z);
            particles.transform.localScale = new Vector3(visibleWorldWidth * WidthMultiplier, 1.0f, 1.0f);
            var m = particles.main;
            m.simulationSpace = ParticleSystemSimulationSpace.Custom;
            m.customSimulationSpace = transform;
            var startSpeed = m.startSpeed;
            startSpeed.constantMin = initialStartSpeed.x * cameraMultiplier;
            startSpeed.constantMax = initialStartSpeed.y * cameraMultiplier;
            var startSizeX = m.startSizeX;
            startSizeX.constantMin = initialStartSize.Key.x * cameraMultiplier;
            startSizeX.constantMax = initialStartSize.Value.x * cameraMultiplier;
            var startSizeY = m.startSizeY;
            startSizeY.constantMin = initialStartSize.Key.y * cameraMultiplier;
            startSizeY.constantMax = initialStartSize.Value.y * cameraMultiplier;
            var startSizeZ = m.startSizeZ;
            startSizeZ.constantMin = initialStartSize.Key.z * cameraMultiplier;
            startSizeZ.constantMax = initialStartSize.Value.z * cameraMultiplier;
            m.startSpeed = startSpeed;
            m.startSizeX = startSizeX;
            m.startSizeY = startSizeY;
            m.startSizeZ = startSizeZ;
        }

        private void EmitExplosion(ref Vector3 pos)
        {
            var em = ExplosionParticleSystem.emission;
            var vel = ExplosionParticleSystem.velocityOverLifetime;
            var velX = vel.x;
            var velY = vel.y;
            Vector3 origPos = ExplosionParticleSystem.transform.position;
            origPos.x = pos.x;
            origPos.y = pos.y;
            ExplosionParticleSystem.transform.position = origPos;
            var m = ExplosionParticleSystem.main;
            var c1 = m.startSpeed;
            float c1Orig = c1.curveMultiplier;
            c1.curveMultiplier = cameraMultiplier;
            var c2 = m.startSize;
            float c2Orig = c2.curveMultiplier;
            var rate = em.rateOverTime;
            c2.curveMultiplier = cameraMultiplier;
            velX.constantMin *= cameraMultiplier;
            velX.constantMax *= cameraMultiplier;
            velY.constantMin *= cameraMultiplier;
            velY.constantMax *= cameraMultiplier;
            ExplosionParticleSystem.Emit(UnityEngine.Random.Range((int)rate.constantMin, (int)rate.constantMax));
            velX.constantMin /= cameraMultiplier;
            velX.constantMax /= cameraMultiplier;
            velY.constantMin /= cameraMultiplier;
            velY.constantMax /= cameraMultiplier;
            c1.curveMultiplier = c1Orig;
            c2.curveMultiplier = c2Orig;
        }

        private void CheckForCollisionsParticles()
        {
            if (ExplosionParticleSystem == null)
            {
                return;
            }

            bool collisionEnabled = (CollisionMask != 0 && CollisionEnabled);
            float dt = Time.deltaTime;
            bool changes = false;
            int count = ParticleSystem.GetParticles(particles);
            RaycastHit2D hit;
            float maxCollisionLifetime = CollisionLifeTime - Mathf.Epsilon;

            for (int i = 0; i < count; i++)
            {
                if (collisionEnabled)
                {
                    if (particles[i].remainingLifetime > CollisionLifeTime)
                    {
                        Vector3 pos = particles[i].position;
                        hit = Physics2D.Raycast(pos, particles[i].velocity.normalized, particles[i].velocity.magnitude * dt, CollisionMask);
                        if (hit.collider != null)
                        {
                            particles[i].position = pos;
                            if (CollisionLifeTime == 0.0f)
                            {
                                particles[i].remainingLifetime = 0.0f;
                                pos = hit.point;
                                EmitExplosion(ref pos);
                            }
                            else
                            {
                                float lifeTime = Mathf.Min(particles[i].remainingLifetime, UnityEngine.Random.Range(0.0f, maxCollisionLifetime));
                                if (lifeTime <= ExplosionEmissionLifeTimeMaximum)
                                {
                                    pos = hit.point;
                                    EmitExplosion(ref pos);
                                }
                                particles[i].remainingLifetime = lifeTime;
                            }
                            changes = true;
                        }
                    }
                    else if (particles[i].remainingLifetime <= ExplosionEmissionLifeTimeMaximum)
                    {
                        Vector3 pos = particles[i].position;
                        EmitExplosion(ref pos);
                    }
                }
                else if (particles[i].remainingLifetime <= ExplosionEmissionLifeTimeMaximum)
                {
                    Vector3 pos = particles[i].position;
                    EmitExplosion(ref pos);
                }
            }

            if (changes)
            {
                ParticleSystem.SetParticles(particles, count);
            }
        }

        private void CheckForCollisionsMistParticles()
        {
            if (MistParticleSystem == null || CollisionMask == 0)
            {
                return;
            }

            bool collisionEnabled = (CollisionMask != 0 && CollisionEnabled);
            if (!collisionEnabled)
            {
                return;
            }

            int count = MistParticleSystem.GetParticles(particles);
            bool changes = false;
            RaycastHit2D hit;

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = particles[i].position;
                hit = Physics2D.Raycast(pos, particles[i].velocity.normalized, particles[i].velocity.magnitude * Time.deltaTime);
                if (hit.collider != null && ((1 << hit.collider.gameObject.layer) & CollisionMask) != 0)
                {
                    particles[i].velocity *= MistCollisionVelocityMultiplier;
                    particles[i].remainingLifetime *= MistCollisionLifeTimeMultiplier;
                    changes = true;
                }
            }

            if (changes)
            {
                MistParticleSystem.SetParticles(particles, count);
            }
        }

        protected override void OnCameraPreCull(Camera camera)
        {

#if UNITY_EDITOR

            if (!Application.isPlaying || camera.cameraType != CameraType.Game)
            {
                return;
            }

#endif

            if (Material != null)
            {
                Material.EnableKeyword("ORTHOGRAPHIC_MODE");
            }
            if (MaterialSecondary != null)
            {
                MaterialSecondary.EnableKeyword("ORTHOGRAPHIC_MODE");
            }
            if (MistMaterial != null)
            {
                MistMaterial.EnableKeyword("ORTHOGRAPHIC_MODE");
            }
            if (ExplosionMaterial != null)
            {
                ExplosionMaterial.EnableKeyword("ORTHOGRAPHIC_MODE");
            }

            cameraMultiplier = (camera.orthographicSize * 0.25f);
            visibleWorldWidth = camera.orthographicSize * 2.0f;
            yOffset = visibleWorldWidth * HeightMultiplier;
            visibleWorldWidth *= camera.aspect;

            TransformParticleSystem(ParticleSystem, camera, InitialStartSpeed, InitialStartSize);
            TransformParticleSystem(ParticleSystemSecondary, camera, InitialStartSpeedSecondary, InitialStartSizeSecondary);
            TransformParticleSystem(MistParticleSystem, camera, InitialStartSpeedMist, InitialStartSizeMist);

            CheckForCollisionsParticles();
            CheckForCollisionsMistParticles();
        }
    }
}