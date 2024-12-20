using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace EswardHunter
{
    public partial class MainWindow : Window
    {
        private bool isDragging = false; // Indica si se está arrastrando un componente
        private UIElement draggedElement; // Referencia al componente que se está moviendo
        private Point dragStartPoint; // Posición inicial del arrastre

        public MainWindow()
        {
            InitializeComponent();
        }

        // Evento para iniciar el arrastre desde el panel de herramientas
        private void Component_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !isDragging)
            {
                isDragging = true;

                // Crea un DataObject con el nombre del componente
                Button draggedButton = sender as Button;
                DataObject dragData = new DataObject("Component", draggedButton.Content.ToString());
                DragDrop.DoDragDrop(draggedButton, dragData, DragDropEffects.Copy);

                // Comentario de prueba

                isDragging = false;
            }
        }

        // Evento para soltar el componente en el Canvas
        private void Canvas_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Component"))
            {
                string componentName = e.Data.GetData("Component") as string;

                // Crear un nuevo botón representando el componente
                Button newComponent = new Button
                {
                    Content = componentName,
                    Background = Brushes.LightBlue
                };

                // Obtener posición del mouse y agregar al Canvas
                Point dropPosition = e.GetPosition(canvasWorkbench);
                Canvas.SetLeft(newComponent, dropPosition.X);
                Canvas.SetTop(newComponent, dropPosition.Y);

                // Habilitar movimiento dentro del Canvas
                newComponent.PreviewMouseLeftButtonDown += Component_PreviewMouseLeftButtonDown;
                newComponent.PreviewMouseMove += Component_PreviewMouseMove;
                newComponent.PreviewMouseLeftButtonUp += Component_PreviewMouseLeftButtonUp;

                canvasWorkbench.Children.Add(newComponent);
            }
        }

        // Evento para capturar el inicio del movimiento de un componente en el Canvas
        private void Component_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            draggedElement = sender as UIElement;
            dragStartPoint = e.GetPosition(canvasWorkbench);
            isDragging = true;

            // Traer el componente al frente (opcional)
            Panel.SetZIndex(draggedElement, canvasWorkbench.Children.Count);
        }

        // Evento para mover el componente dentro del Canvas
        private void Component_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && draggedElement != null && e.LeftButton == MouseButtonState.Pressed)
            {
                // Calcular la nueva posición
                Point currentPosition = e.GetPosition(canvasWorkbench);
                double offsetX = currentPosition.X - dragStartPoint.X;
                double offsetY = currentPosition.Y - dragStartPoint.Y;

                // Obtener las coordenadas actuales del elemento
                double currentLeft = Canvas.GetLeft(draggedElement);
                double currentTop = Canvas.GetTop(draggedElement);

                // Actualizar la posición
                Canvas.SetLeft(draggedElement, currentLeft + offsetX);
                Canvas.SetTop(draggedElement, currentTop + offsetY);

                // Actualizar el punto inicial para el siguiente movimiento
                dragStartPoint = currentPosition;
            }
        }

        // Evento para soltar el componente después del movimiento
        private void Component_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            draggedElement = null;
        }
    }
}

