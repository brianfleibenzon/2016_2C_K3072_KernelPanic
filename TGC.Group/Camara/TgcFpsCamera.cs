using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Utils;
using TGC.Core.SceneLoader;
using TGC.Core.Collision;
using TGC.Core.BoundingVolumes;
using System.Collections.Generic;

namespace TGC.Group.Camara
{
    /// <summary>
    ///     Camara en primera persona que utiliza matrices de rotacion, solo almacena las rotaciones en updown y costados.
    ///     Ref: http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series4/Mouse_camera.php
    ///     Autor: Rodrigo Garcia.
    /// </summary>
    public class TgcFpsCamera : TgcCamera
    {
        private readonly Point mouseCenter; //Centro de mause 2D para ocultarlo.

        //Se mantiene la matriz rotacion para no hacer este calculo cada vez.
        private Matrix cameraRotation;

        //Direction view se calcula a partir de donde se quiere ver con la camara inicialmente. por defecto se ve en -Z.
        private Vector3 directionView;        

        //No hace falta la base ya que siempre es la misma, la base se arma segun las rotaciones de esto costados y updown.
        private float leftrightRot;
        private float updownRot;

        private bool lockCam;
        private Vector3 positionEye;

        public bool colisiones;

        TgcScene scene;

        public TgcFpsCamera(TgcD3dInput input)
        {
            Input = input;
            positionEye = new Vector3();
            mouseCenter = new Point(
                D3DDevice.Instance.Device.Viewport.Width / 2,
                D3DDevice.Instance.Device.Viewport.Height / 2);
            RotationSpeed = 0.1f;
            MovementSpeed = 250f;
            JumpSpeed = 500f;
            directionView = new Vector3(0, 0, -1);
            leftrightRot = FastMath.PI;
            //updownRot = -FastMath.PI / 10.0f;
            cameraRotation = Matrix.RotationX(updownRot) * Matrix.RotationY(leftrightRot);
        }


        public TgcFpsCamera(Vector3 positionEye, TgcD3dInput input) : this(input)
        {
            this.positionEye = positionEye;
        }

        public TgcFpsCamera(TgcScene scene, Vector3 positionEye, TgcD3dInput input) : this(positionEye, input)
        {
            this.scene = scene;
        }

        public TgcFpsCamera(Vector3 positionEye, float moveSpeed, float jumpSpeed, TgcD3dInput input)
            : this(positionEye, input)
        {
            MovementSpeed = moveSpeed;
            JumpSpeed = jumpSpeed;
        }

        public TgcFpsCamera(Vector3 positionEye, float moveSpeed, float jumpSpeed, float rotationSpeed,
            TgcD3dInput input)
            : this(positionEye, moveSpeed, jumpSpeed, input)
        {
            RotationSpeed = rotationSpeed;
        }

        private TgcD3dInput Input { get; }

        public bool LockCam
        {
            get { return lockCam; }
            set
            {
                if (!lockCam && value)
                {
                    Cursor.Position = mouseCenter;

                    Cursor.Hide();
                }
                if (lockCam && !value)
                    Cursor.Show();
                lockCam = value;
            }
        }

        public float MovementSpeed { get; set; }

        public float RotationSpeed { get; set; }

        public float JumpSpeed { get; set; }

        /// <summary>
        ///     Cuando se elimina esto hay que desbloquear la camera.
        /// </summary>
        ~TgcFpsCamera()
        {
            LockCam = false;
        }

        public override void UpdateCamera(float elapsedTime)
        {
            Vector3 lastPositionEye = positionEye;

            var moveVector = new Vector3(0, 0, 0);
            //Forward
            if (Input.keyDown(Key.W))
            {
                moveVector += new Vector3(0, 0, -1) * MovementSpeed;
            }

            //Backward
            if (Input.keyDown(Key.S))
            {
                moveVector += new Vector3(0, 0, 1) * MovementSpeed;
            }

            //Strafe right
            if (Input.keyDown(Key.D))
            {

                /* SI MUEVE LA POSICION*/
                moveVector += new Vector3(-1, 0, 0) * MovementSpeed;

                /* SI ROTA LA CAMARA*/
                /*leftrightRot += 0.1f * RotationSpeed;
                cameraRotation = Matrix.RotationX(updownRot) * Matrix.RotationY(leftrightRot);*/
            }

            //Strafe left
            if (Input.keyDown(Key.A))
            {
                /* SI MUEVE LA POSICION*/

                moveVector += new Vector3(1, 0, 0) * MovementSpeed;

                /* SI ROTA LA CAMARA*/
                /*leftrightRot -= 0.1f * RotationSpeed;
                cameraRotation = Matrix.RotationX(updownRot) * Matrix.RotationY(leftrightRot);*/
            }   


            if (Input.keyPressed(Key.L) || Input.keyPressed(Key.Escape))
            {
                LockCam = !lockCam;
            }

            //Solo rotar si se esta aprentando el boton izq del mouse
            if (lockCam || Input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                leftrightRot -= -Input.XposRelative * RotationSpeed;
                //updownRot -= Input.YposRelative * RotationSpeed;
                //Se actualiza matrix de rotacion, para no hacer este calculo cada vez y solo cuando en verdad es necesario.
                cameraRotation = Matrix.RotationX(updownRot) * Matrix.RotationY(leftrightRot);
            }

            if (lockCam)
                Cursor.Position = mouseCenter;

            if (Input.keyPressed(Key.C))
            {
                colisiones = !colisiones;
            }

            //Calculamos la nueva posicion del ojo segun la rotacion actual de la camara.
            var cameraRotatedPositionEye = Vector3.TransformNormal(moveVector * elapsedTime, cameraRotation);
            positionEye += cameraRotatedPositionEye;

            //Calculamos el target de la camara, segun su direccion inicial y las rotaciones en screen space x,y.
            var cameraRotatedTarget = Vector3.TransformNormal(directionView, cameraRotation);
            var cameraFinalTarget = positionEye + cameraRotatedTarget;

            var cameraOriginalUpVector = DEFAULT_UP_VECTOR;
            var cameraRotatedUpVector = Vector3.TransformNormal(cameraOriginalUpVector, cameraRotation);




            bool colision = false;

            if (colisiones)
            {
                foreach (var mesh in scene.Meshes)
                {
                    if (TgcCollisionUtils.sqDistPointAABB(positionEye, mesh.BoundingBox) < 100f)
                    {
                        colision = true;
                        break;
                    }
                }
            }
            if (colision)
                positionEye = lastPositionEye;
            else
                base.SetCamera(positionEye, cameraFinalTarget, cameraRotatedUpVector);
            
        }

        /// <summary>
        ///     se hace override para actualizar las posiones internas, estas seran utilizadas en el proximo update.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="directionView"> debe ser normalizado.</param>
        public override void SetCamera(Vector3 position, Vector3 directionView)
        {

            positionEye = position;       
            
            this.directionView = directionView;
        }

    }
}