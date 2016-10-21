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
using TGC.Core.Geometry;
using TGC.Group.Model;

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

        private Matrix cameraRotationParcial;

        //Direction view se calcula a partir de donde se quiere ver con la camara inicialmente. por defecto se ve en -Z.
        private Vector3 directionView;        

        //No hace falta la base ya que siempre es la misma, la base se arma segun las rotaciones de esto costados y updown.
        private float leftrightRot;
        private float updownRot;

        private bool lockCam;
        private Vector3 positionEye;

        public bool colisiones = true;

        public bool agachado = false;

        GameModel gameModel;

        public TgcBoundingAxisAlignBox camaraBox = new TgcBoundingAxisAlignBox();


        public TgcFpsCamera(TgcD3dInput input)
        {
            Input = input;
            positionEye = new Vector3();
            mouseCenter = new Point(
                D3DDevice.Instance.Device.Viewport.Width / 2,
                D3DDevice.Instance.Device.Viewport.Height / 2);
            RotationSpeed = 0.1f;
            MovementSpeed = 550f;
            JumpSpeed = 500f;
            directionView = new Vector3(0, 0, -1);
            leftrightRot = FastMath.PI;
            cameraRotationParcial = Matrix.RotationY(leftrightRot);
            cameraRotation = Matrix.RotationX(updownRot) * Matrix.RotationY(leftrightRot);
        }


        public TgcFpsCamera(Vector3 positionEye, TgcD3dInput input) : this(input)
        {
            this.positionEye = positionEye;
        }

        public TgcFpsCamera(GameModel gameModel, Vector3 positionEye, TgcD3dInput input) : this(positionEye, input)
        {
            this.gameModel = gameModel;
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

            if (Input.keyPressed(Key.LeftControl))
            {
                if (agachado)
                {
                    
                    lastPositionEye.Y = 90f;
                    positionEye.Y = 90f;
                    //moveVector += new Vector3(0, 8000f, 0);
                    MovementSpeed = 250f;
                }
                else
                {
                    lastPositionEye.Y = 40f;
                    positionEye.Y = 40f;
                    //moveVector += new Vector3(0, -8000f, 0);
                    MovementSpeed = 80f;
                }
                agachado = !agachado;
            }


            /*if (Input.keyPressed(Key.L) || Input.keyPressed(Key.Escape))
            {
                LockCam = !lockCam;
            }*/

            //Solo rotar si se esta aprentando el boton izq del mouse
            if (lockCam || Input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                leftrightRot -= -Input.XposRelative * RotationSpeed;
                updownRot -= Input.YposRelative * RotationSpeed;
                //Se actualiza matrix de rotacion, para no hacer este calculo cada vez y solo cuando en verdad es necesario.
                cameraRotationParcial = Matrix.RotationY(leftrightRot);
                cameraRotation = Matrix.RotationX(updownRot) * Matrix.RotationY(leftrightRot);
            }

            if (lockCam)
                Cursor.Position = mouseCenter;

            if (Input.keyPressed(Key.C))
            {
                colisiones = !colisiones;
            }

            //Calculamos la nueva posicion del ojo segun la rotacion actual de la camara.
            //var cameraRotatedPositionEye = Vector3.TransformNormal(moveVector * elapsedTime, cameraRotation);
            //Uso cameraRotationPacial para que no pueda moverse para arriba y para abajo
            var cameraRotatedPositionEye = Vector3.TransformNormal(moveVector * elapsedTime, cameraRotationParcial);
            positionEye += cameraRotatedPositionEye;

            if (colisiones)
            {
                Vector3 pMin = new Vector3(positionEye.X - 10f, 10f, positionEye.Z - 10f);
                Vector3 pMax = new Vector3(positionEye.X + 10f, positionEye.Y + 5f, positionEye.Z + 10f);
                
                camaraBox.setExtremes(pMin, pMax);
                
                foreach (var mesh in gameModel.meshesARenderizar)
                {
                    /* COLISIONES POR RAYOS*/
                    /*if (TgcCollisionUtils.sqDistPointAABB(positionEye, mesh.BoundingBox) < 100f)
                    {
                        colision = true;
                        break;
                    }*/
                    if (TgcCollisionUtils.classifyBoxBox(camaraBox, mesh.BoundingBox) == TgcCollisionUtils.BoxBoxResult.Atravesando)
                    {
                        if (!gameModel.VerificarSiMeshEsIluminacion(mesh)){
                            positionEye = lastPositionEye;
                            break;
                        }
                    }
                }

                foreach (var enemigo in gameModel.enemigos)
                {
                    if (TgcCollisionUtils.classifyBoxBox(camaraBox, enemigo.mesh.BoundingBox) == TgcCollisionUtils.BoxBoxResult.Atravesando)
                    { 
                        positionEye = lastPositionEye;
                        break;
                        
                    }
                }
            }


            //Calculamos el target de la camara, segun su direccion inicial y las rotaciones en screen space x,y.
            var cameraRotatedTarget = Vector3.TransformNormal(directionView, cameraRotation);
            var cameraFinalTarget = positionEye + cameraRotatedTarget;

            var cameraOriginalUpVector = DEFAULT_UP_VECTOR;
            var cameraRotatedUpVector = Vector3.TransformNormal(cameraOriginalUpVector, cameraRotation);

            
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